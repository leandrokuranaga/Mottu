using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Moq;
using Mottu.Application.Courier.Models.Request;
using Mottu.Application.Courier.Services;
using Mottu.Domain.SeedWork;
using Mottu.Domain.UserAggregate;
using Mottu.Infra.Storage;

namespace Mottu.Unit.Tests.Application.Courier
{
    public class UserServiceTests
    {
        private readonly Mock<INotification> _notification = new();
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IObjectStorage> _storage = new();

        private UserService CreateSut() =>
            new UserService(_notification.Object, _userRepo.Object, _uow.Object, _storage.Object);

        [Fact]
        public async Task CreateCourier_ShouldCreate_WhenUniqueCNPJAndCNH()
        {
            var svc = CreateSut();

            _userRepo.Setup(r => r.GetOneNoTracking(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User?)null);

            _userRepo.Setup(r => r.InsertOrUpdateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => { u.Id = 55; return u; });

            _userRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var req = new CreateCourierRequest
            {
                Name = "João",
                BirthdayDate = new DateOnly(1990, 1, 1),
                CNPJ = "04252011000110",
                CNH = "12345678900",
                TypeCNH = Mottu.Domain.UserAggregate.Enums.ECNH.AB
            };

            var res = await svc.CreateCourier(req);

            Assert.NotNull(res);

            _userRepo.Verify(r => r.InsertOrUpdateAsync(It.IsAny<User>()), Times.Once);
            _userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UploadCNHPhoto_ShouldFail_WhenFileEmpty()
        {
            var svc = CreateSut();

            var file = new Mock<IFormFile>();
            file.SetupGet(f => f.Length).Returns(0);

            var res = await svc.UploadCNHPhoto(1, file.Object);

            Assert.False(res.Success);
            _notification.Verify(n => n.AddNotification(
                "Upload CNH", "File is empty", NotificationModel.ENotificationType.BusinessRules), Times.Once);
        }

        [Fact]
        public async Task UploadCNHPhoto_ShouldFail_WhenContentTypeInvalid()
        {
            var svc = CreateSut();

            var file = new Mock<IFormFile>();
            file.SetupGet(f => f.Length).Returns(10);
            file.SetupGet(f => f.ContentType).Returns("image/jpeg");
            file.SetupGet(f => f.FileName).Returns("photo.jpg");

            var res = await svc.UploadCNHPhoto(1, file.Object);

            Assert.False(res.Success);
            _notification.Verify(n => n.AddNotification(
                "Upload CNH", "Only bmp or png are accepted", NotificationModel.ENotificationType.BusinessRules), Times.Once);
        }

        [Fact]
        public async Task UploadCNHPhoto_ShouldFail_WhenExtensionInvalid()
        {
            var svc = CreateSut();

            var file = new Mock<IFormFile>();
            file.SetupGet(f => f.Length).Returns(10);
            file.SetupGet(f => f.ContentType).Returns("image/png");
            file.SetupGet(f => f.FileName).Returns("photo.jpg");

            var res = await svc.UploadCNHPhoto(1, file.Object);

            Assert.False(res.Success);
            _notification.Verify(n => n.AddNotification(
                "Upload CNH", "Only bmp or png are accepted", NotificationModel.ENotificationType.BusinessRules), Times.Once);
        }

        [Fact]
        public async Task UploadCNHPhoto_ShouldNotify_WhenCourierNotFound()
        {
            var svc = CreateSut();

            var file = new Mock<IFormFile>();
            file.SetupGet(f => f.Length).Returns(10);
            file.SetupGet(f => f.ContentType).Returns("image/png");
            file.SetupGet(f => f.FileName).Returns("photo.png");

            _userRepo.Setup(r => r.GetOneTracking(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User?)null);

            var res = await svc.UploadCNHPhoto(1, file.Object);

            Assert.False(res.Success);
            _notification.Verify(n => n.AddNotification(
                "Get Courier", "Courier not found", NotificationModel.ENotificationType.NotFound), Times.Once);
        }

        [Fact]
        public async Task UploadCNHPhoto_ShouldUpload_AndSave()
        {
            var svc = CreateSut();

            var file = new Mock<IFormFile>();
            file.SetupGet(f => f.Length).Returns(10);
            file.SetupGet(f => f.ContentType).Returns("image/png");
            file.SetupGet(f => f.FileName).Returns("photo.png");

            var user = new User { Id = 7 };

            _userRepo
                .Setup(r => r.GetOneTracking(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);

            _userRepo
                .Setup(r => r.InsertOrUpdateAsync(user))
                .ReturnsAsync(user);

            _userRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _storage
                .Setup(s => s.UploadAsync(
                    file.Object,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("couriers/7/cnh.jpg");

            var res = await svc.UploadCNHPhoto(7, file.Object);

            Assert.True(res.Success);
            Assert.Equal("couriers/7/cnh.jpg", user.CnhImageUri);

            _storage.Verify(s => s.UploadAsync(
                file.Object,
                "couriers/7/cnh.jpg",
                It.IsAny<CancellationToken>()), Times.Once);

            _userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

    }
}
