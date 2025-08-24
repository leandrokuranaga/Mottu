using System;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using Mottu.Application.Rent.Models.Request;
using Mottu.Application.Rent.Services;
using Mottu.Domain.MotorcycleAggregate;
using Mottu.Domain.RentalAggregate;
using Mottu.Domain.RentalAggregate.Enums;
using Mottu.Domain.SeedWork;
using Mottu.Domain.UserAggregate;
using Xunit;

namespace Mottu.Unit.Tests.Application.Rent
{
    public class RentServiceTests
    {
        private readonly Mock<INotification> _notification = new();
        private readonly Mock<IRentalRepository> _rentRepo = new();
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IMotorcycleRepository> _motoRepo = new();

        private RentService CreateSut() =>
            new RentService(_notification.Object, _rentRepo.Object, _userRepo.Object, _uow.Object, _motoRepo.Object);

        [Fact]
        public async Task GetMotorcycleRent_ShouldReturn_WhenFound()
        {
            var svc = CreateSut();
            var rent = Rental.Create(1, 2, ERentalPlan.Days7);
            rent.Id = 33;

            _rentRepo
                .Setup(r => r.GetOneNoTracking(It.IsAny<Expression<Func<Rental, bool>>>()))
                .ReturnsAsync(rent);

            var res = await svc.GetMotorcycleRent(33);

            Assert.NotNull(res);
            Assert.Equal(33, res.Id);
        }

        [Fact]
        public async Task GetMotorcycleRent_ShouldNotify_WhenNotFound()
        {
            var svc = CreateSut();

            _rentRepo
                .Setup(r => r.GetOneNoTracking(It.IsAny<Expression<Func<Rental, bool>>>()))
                .ReturnsAsync((Rental?)null);

            var res = await svc.GetMotorcycleRent(404);

            Assert.NotNull(res);
            Assert.Equal(0, res.Id);
            _notification.Verify(n => n.AddNotification(
                "Get Rent", "Rent not found", NotificationModel.ENotificationType.NotFound), Times.Once);
        }

        [Fact]
        public async Task ReturnMotorcycle_ShouldCompute_AndReturnPayload()
        {
            var svc = CreateSut();

            var rental = new Rental
            {
                Id = 123,
                Plan = ERentalPlan.Days7,
                DailyPrice = new Mottu.Domain.RentalAggregate.ValueObjects.Money(100m),
                StartDate = new DateOnly(2025, 08, 24),
                ForecastEndDate = new DateOnly(2025, 08, 30),
                Status = ERentalStatus.Pending
            };

            _rentRepo.Setup(r => r.GetByIdAsync(123, false)).ReturnsAsync(rental);
            _rentRepo.Setup(r => r.UpdateAsync(It.IsAny<Rental>())).Returns(Task.CompletedTask);
            _rentRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var retDate = new DateOnly(2025, 08, 30);

            var res = await svc.ReturnMotorcycle(123, retDate);

            Assert.True(res.Success);

            // Valida payload via JSON (robusto contra tipos anônimos)
            var json = JsonSerializer.Serialize(res.Data);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Assert.Equal(123, root.GetProperty("rentalId").GetInt32());
            Assert.Equal("Days7", root.GetProperty("plan").GetString());
            Assert.Equal(7, root.GetProperty("planDays").GetInt32());
            Assert.Equal(700m, root.GetProperty("total").GetDecimal());
            Assert.Equal(700m, root.GetProperty("dailyBasis").GetDecimal());
            Assert.Equal(0m, root.GetProperty("feeOrExtra").GetDecimal());
            Assert.False(root.GetProperty("isEarly").GetBoolean());
            Assert.False(root.GetProperty("isLate").GetBoolean());

            _rentRepo.Verify(r => r.UpdateAsync(It.IsAny<Rental>()), Times.Once);
            _rentRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ReturnMotorcycle_ShouldNotify_WhenNotFound()
        {
            var svc = CreateSut();

            _rentRepo
                .Setup(r => r.GetByIdAsync(999, false))
                .ReturnsAsync((Rental?)null);

            var res = await svc.ReturnMotorcycle(999, new DateOnly(2025, 08, 25));

            Assert.False(res.Success);
            _notification.Verify(n => n.AddNotification(
                "Return Rental", "Rental not found", NotificationModel.ENotificationType.NotFound), Times.Once);
        }
    }
}
