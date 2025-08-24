using System.Linq.Expressions;
using System.Text.Json;
using Moq;
using Mottu.Application.Motorcycle.Models.Request;
using Mottu.Application.Motorcycle.Services;
using Mottu.Domain.MotorcycleAggregate;
using Mottu.Domain.OutboxAggregate;
using Mottu.Domain.RentalAggregate;
using Mottu.Domain.SeedWork;
using DomainMotorcycle = Mottu.Domain.MotorcycleAggregate.Motorcycle;

namespace Mottu.Unit.Tests.Application.Motorcycle
{
    public class MotorcycleServiceTests
    {
        private readonly Mock<INotification> _notification = new();
        private readonly Mock<IMotorcycleRepository> _motoRepo = new();
        private readonly Mock<IOutboxRepository> _outboxRepo = new();
        private readonly Mock<IRentalRepository> _rentalRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        private MotorcycleService CreateSut() =>
            new MotorcycleService(_notification.Object, _motoRepo.Object, _outboxRepo.Object, _rentalRepo.Object, _uow.Object);

        [Fact]
        public async Task CreateMotorcycle_ShouldCreate_Outbox_AndReturnResponse()
        {
            // Arrange
            var svc = CreateSut();

            var req = new CreateMotorcycleRequest
            {
                Year = DateTime.UtcNow.Year,
                Brand = "Honda",
                LicensePlate = "ABC1234"
            };

            _motoRepo
                .Setup(r => r.GetOneNoTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync((DomainMotorcycle?)null);

            Outbox? captured = null;
            _outboxRepo
                .Setup(r => r.InsertOrUpdateAsync(It.IsAny<Outbox>()))
                .Callback<Outbox>(o => captured = o)
                .ReturnsAsync((Outbox o) => o);

            _outboxRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var res = await svc.CreateMotorcycle(req);

            // Assert
            Assert.NotNull(res);
            Assert.Equal(req.Brand, res.Brand);
            Assert.Equal(req.Year, res.Year);
            Assert.Equal(req.LicensePlate, res.LicensePlate);

            _motoRepo.Verify(r => r.GetOneNoTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()), Times.Once);
            _outboxRepo.Verify(r => r.InsertOrUpdateAsync(It.IsAny<Outbox>()), Times.Once);
            _outboxRepo.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.NotNull(captured);
            Assert.Equal("plain-json", captured!.Type);
            using var doc = JsonDocument.Parse(captured.Content);
            Assert.Equal("object", doc.RootElement.GetProperty("type").GetString());
            Assert.Equal(1, doc.RootElement.GetProperty("version").GetInt32());
        }

        [Fact]
        public async Task CreateMotorcycle_ShouldAddNotification_WhenLicenseAlreadyExists()
        {
            // Arrange
            var svc = CreateSut();
            var req = new CreateMotorcycleRequest
            {
                Year = DateTime.UtcNow.Year,
                Brand = "Honda",
                LicensePlate = "ABC1234"
            };

            _motoRepo
                .Setup(r => r.GetOneNoTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync(new DomainMotorcycle());

            // Act
            var res = await svc.CreateMotorcycle(req);

            // Assert
            Assert.NotNull(res);
            _notification.Verify(n => n.AddNotification(
                "Create Motorcycle",
                "License plate already registered",
                NotificationModel.ENotificationType.BusinessRules), Times.Once);

            _outboxRepo.Verify(r => r.InsertOrUpdateAsync(It.IsAny<Outbox>()), Times.Never);
            _outboxRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetMotorcycle_ShouldReturnResponse_WhenFound()
        {
            var svc = CreateSut();
            var moto = DomainMotorcycle.Create(DateTime.UtcNow.Year, "Yamaha", "XYZ1A23");
            moto.Id = 10;

            _motoRepo
                .Setup(r => r.GetOneNoTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync(moto);

            var res = await svc.GetMotorcycle(10);

            Assert.NotNull(res);
            Assert.Equal("Yamaha", res.Brand);
        }

        [Fact]
        public async Task GetMotorcycle_ShouldNotifyNotFound_WhenNull()
        {
            var svc = CreateSut();

            _motoRepo
                .Setup(r => r.GetOneNoTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync((DomainMotorcycle?)null);

            var res = await svc.GetMotorcycle(99);

            Assert.NotNull(res);

            _notification.Verify(n => n.AddNotification(
                "Get Motorcycle",
                "Motorcycle not found",
                NotificationModel.ENotificationType.NotFound), Times.Once);
        }

        [Fact]
        public async Task GetMotorcycles_FilterByPlate_ShouldReturnMatches()
        {
            var svc = CreateSut();
            var list = new List<DomainMotorcycle>
            {
                DomainMotorcycle.Create(2024, "Honda", "ABC1234"),
                DomainMotorcycle.Create(2023, "Yamaha", "ABC1234"),
            };

            _motoRepo
                .Setup(r => r.GetNoTrackingAsync(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync(list);

            var res = await svc.GetMotorcycles("ABC1234");

            Assert.Equal(2, res.Count);
            Assert.All(res, x => Assert.Equal("ABC1234", x.LicensePlate));
        }

        [Fact]
        public async Task GetMotorcycles_FilterByPlate_ShouldNotify_WhenNone()
        {
            var svc = CreateSut();

            _motoRepo
                .Setup(r => r.GetNoTrackingAsync(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync(new List<DomainMotorcycle>());

            var res = await svc.GetMotorcycles("ZZZ9999");

            Assert.Empty(res);
            _notification.Verify(n => n.AddNotification(
                "Get Motorcycle",
                "License plate doesn't exist",
                NotificationModel.ENotificationType.NotFound), Times.Once);
        }

        [Fact]
        public async Task GetMotorcycles_NoFilter_ShouldReturnAll()
        {
            var svc = CreateSut();
            var list = new List<DomainMotorcycle>
            {
                DomainMotorcycle.Create(2024, "Honda", "ABC1234"),
                DomainMotorcycle.Create(2023, "Yamaha", "XYZ1A23"),
            };

            _motoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var res = await svc.GetMotorcycles(null);

            Assert.Equal(2, res.Count);
        }

        [Fact]
        public async Task UpdateLicensePlate_ShouldUpdate_WhenFound()
        {
            var svc = CreateSut();
            var m = DomainMotorcycle.Create(2024, "Honda", "AAA1234");
            m.Id = 5;

            _motoRepo
                .Setup(r => r.GetOneTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync(m);

            _motoRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var res = await svc.UpdateLicensePlate(5, "BBB1234");

            Assert.True(res.Success);
            _motoRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.Equal("BBB1234", m.LicensePlate.Value);
        }

        [Fact]
        public async Task UpdateLicensePlate_ShouldNotify_WhenNotFound()
        {
            var svc = CreateSut();

            _motoRepo
                .Setup(r => r.GetOneTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync((DomainMotorcycle?)null);

            var res = await svc.UpdateLicensePlate(999, "CCC1234");

            Assert.False(res.Success);
            _notification.Verify(n => n.AddNotification(
                "Update Motorcycle",
                "Motorcycle not found",
                NotificationModel.ENotificationType.NotFound), Times.Once);
        }

        [Fact]
        public async Task DeleteMotorcycle_ShouldDelete_WhenNoRentals()
        {
            var svc = CreateSut();

            var m = DomainMotorcycle.Create(2024, "Honda", "AAA1234");
            m.Id = 9;

            _motoRepo
                .Setup(r => r.GetOneTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync(m);

            _rentalRepo
                .Setup(r => r.GetNoTrackingAsync(It.IsAny<Expression<Func<Rental, bool>>>()))
                .ReturnsAsync(new List<Rental>());

            _motoRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var res = await svc.DeleteMotorcycle(9);

            Assert.True(res.Success);
            _motoRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.True(m.IsDeleted);
        }

        [Fact]
        public async Task DeleteMotorcycle_ShouldNotify_WhenHasRentals()
        {
            var svc = CreateSut();

            var m = DomainMotorcycle.Create(2024, "Honda", "AAA1234");
            m.Id = 9;

            _motoRepo
                .Setup(r => r.GetOneTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync(m);

            _rentalRepo
                .Setup(r => r.GetNoTrackingAsync(It.IsAny<Expression<Func<Rental, bool>>>()))
                .ReturnsAsync(new List<Rental> { new Rental() });

            var res = await svc.DeleteMotorcycle(9);

            Assert.False(res.Success);
            _notification.Verify(n => n.AddNotification(
                "Delete Motorcycle",
                "Motorcycle has rental entries and cannot be deleted",
                NotificationModel.ENotificationType.BusinessRules), Times.Once);
        }

        [Fact]
        public async Task DeleteMotorcycle_ShouldNotify_WhenNotFound()
        {
            var svc = CreateSut();

            _motoRepo
                .Setup(r => r.GetOneTracking(It.IsAny<Expression<Func<DomainMotorcycle, bool>>>()))
                .ReturnsAsync((DomainMotorcycle?)null);

            var res = await svc.DeleteMotorcycle(123);

            Assert.False(res.Success);
            _notification.Verify(n => n.AddNotification(
                "Get Motorcycle",
                "Motorcycle not found",
                NotificationModel.ENotificationType.NotFound), Times.Once);
        }
    }
}
