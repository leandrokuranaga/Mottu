using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Mottu.Application.Motorcycle.Models.Dto;
using Mottu.Application.Motorcycle.Services;
using Mottu.Domain.MotorcycleAggregate;
using DomainMotorcycle = Mottu.Domain.MotorcycleAggregate.Motorcycle;

namespace Mottu.Unit.Tests.Application.Motorcycle
{
    public class MotorcycleRegisteredHandlerTests
    {
        private readonly Mock<IMotorcycleRepository> _repo = new();
        private readonly Mock<ILogger<string>> _logger = new();

        private MotorcycleRegisteredHandler CreateSut() => new(_repo.Object, _logger.Object);

        [Fact]
        public async Task Handle_ShouldInsert_WhenEnvelopeValid()
        {
            var sut = CreateSut();

            var payload = new
            {
                type = "object",
                version = 1,
                data = new CreateMotorcycleMessage
                {
                    Id = 42,
                    Year = 2024,
                    Brand = "Honda",
                    LicensePlate = "ABC1234"
                }
            };

            var json = JsonSerializer.Serialize(payload);

            _repo.Setup(r => r.InsertOrUpdateAsync(It.IsAny<DomainMotorcycle>()))
                .ReturnsAsync((DomainMotorcycle m) => m);
            _repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            await sut.Handle(json);

            _repo.Verify(r => r.InsertOrUpdateAsync(It.IsAny<DomainMotorcycle>()), Times.Once);
            _repo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenJsonInvalid()
        {
            var sut = CreateSut();
            var invalid = "{ not a json";

            await sut.Handle(invalid);

            _logger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<System.Exception>(),
                It.IsAny<System.Func<It.IsAnyType, System.Exception?, string>>()
            ), Times.AtLeastOnce);

            _repo.Verify(r => r.InsertOrUpdateAsync(It.IsAny<DomainMotorcycle>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldIgnore_WhenTypeOrVersionUnexpected()
        {
            var sut = CreateSut();

            var payload = new
            {
                type = "something-else",
                version = 999,
                data = new CreateMotorcycleMessage
                {
                    Id = 1,
                    Year = 2023,
                    Brand = "Yamaha",
                    LicensePlate = "XYZ1A23"
                }
            };
            var json = JsonSerializer.Serialize(payload);

            await sut.Handle(json);

            _repo.Verify(r => r.InsertOrUpdateAsync(It.IsAny<DomainMotorcycle>()), Times.Never);
        }
    }
}
