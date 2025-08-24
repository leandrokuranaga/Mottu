using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Mottu.Api.Controllers;
using Mottu.Application.Common;
using Mottu.Application.Courier.Models.Request;
using Mottu.Application.Courier.Services;
using Mottu.Application.Motorcycle.Models.Response;
using Mottu.Application.User.Models.Response;
using Mottu.Domain.SeedWork;
using System.Text;

namespace Mottu.Unit.Tests._1._Api_Layer_Tests
{
    public class CouriersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<INotification> _notificationMock;
        private readonly CouriersController _controller;

        public CouriersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _notificationMock = new Mock<INotification>();

            _controller = new CouriersController(_userServiceMock.Object, _notificationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData()
                }
            };
            _controller.ControllerContext.RouteData.Values["controller"] = "couriers";
            _controller.ControllerContext.RouteData.Values["version"] = "1";
        }

        #region CreateCourier

        [Fact]
        public async Task CreateCourier_ShouldReturnCreated_WhenServiceReturnsSuccess()
        {
            // Arrange
            var request = new CreateCourierRequest
            {
                Name = "João",
                CNPJ = "12345678000195",
                BirthdayDate = new DateOnly(1998, 8, 20),
                CNH = "12345678900",
                TypeCNH = Mottu.Domain.UserAggregate.Enums.ECNH.A
            };

            var response = new CourierResponse
            {
                Id = 2,
                Name = "Courier One",
                Role = 0,
                BirthDate = new DateOnly(1995, 05, 10),
                CreatedAtUtc = DateTime.SpecifyKind(new DateTime(2025, 1, 1, 0, 0, 0), DateTimeKind.Utc),
                UpdatedAtUtc = null,
                Cnpj = "12345678000195",
                CnhNumber = "12345678901",
                CnhType = "A",
                CnhImageUri = "courier1-cnh.png"
            };

            _userServiceMock
                .Setup(s => s.CreateCourier(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.CreateCourierAsync(request);

            // Assert
            var ok = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, ok.StatusCode);

            _userServiceMock.Verify(s => s.CreateCourier(request), Times.Once);
        }

        #endregion

        #region GetCourierById

        [Fact]
        public async Task GetCourier_ShouldReturnOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var request = new CreateCourierRequest
            {
                Name = "Courier One",
                CNPJ = "12345678000195",
                BirthdayDate = new DateOnly(1998, 8, 20),
                CNH = "12345678900",
                TypeCNH = Mottu.Domain.UserAggregate.Enums.ECNH.A
            };

            var response = new CourierResponse
            {
                Id = 2,
                Name = "Courier One",
                Role = 0,
                BirthDate = new DateOnly(1995, 05, 10),
                CreatedAtUtc = DateTime.SpecifyKind(new DateTime(2025, 1, 1, 0, 0, 0), DateTimeKind.Utc),
                UpdatedAtUtc = null,
                Cnpj = "12345678000195",
                CnhNumber = "12345678901",
                CnhType = "A",
                CnhImageUri = "courier1-cnh.png"
            };

            _userServiceMock
                .Setup(s => s.GetCourier(2))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetAsync(2);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var baseResponse = Assert.IsType<BaseResponse<CourierResponse>>(ok.Value);
            Assert.True(baseResponse.Success);
            Assert.Equal(2, baseResponse.Data.Id);
        }     

        #endregion

        #region Upload CNH (PATCH)

        [Fact]
        public async Task UploadCnh_ShouldReturnOk_WhenServiceReturnsSuccess()
        {
            // Arrange
            var courierId = 10;

            var bytes = Encoding.UTF8.GetBytes("fake image bytes");
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, bytes.Length, "file", "cnh.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };

            _userServiceMock
                .Setup(s => s.UploadCNHPhoto(courierId, file))
                .ReturnsAsync(BaseResponse<object>.Ok(null));

            // Act
            var result = await _controller.UploadCNHAsync(courierId, file);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _userServiceMock.Verify(s => s.UploadCNHPhoto(courierId, file), Times.Once);
        }

        #endregion
    }
}
