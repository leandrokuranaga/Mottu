using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using Mottu.Api.Controllers;
using Mottu.Application.Common;
using Mottu.Application.Motorcycle.Models.Request;
using Mottu.Application.Motorcycle.Models.Response;
using Mottu.Application.Motorcycle.Services;
using Mottu.Domain.SeedWork;
using System.Net;

namespace Mottu.Unit.Tests._1._Api_Layer_Tests
{
    public class MotorcyclesControllerTests
    {
        private readonly Mock<IMotorcycleService> _serviceMock;
        private readonly Mock<INotification> _notificationMock;
        private readonly MotorcyclesController _controller;

        public MotorcyclesControllerTests()
        {
            _serviceMock = new Mock<IMotorcycleService>();
            _notificationMock = new Mock<INotification>();

            _controller = new MotorcyclesController(_serviceMock.Object, _notificationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext(),
                    RouteData = new RouteData()
                }
            };
            _controller.ControllerContext.RouteData.Values["controller"] = "motorcycles";
            _controller.ControllerContext.RouteData.Values["version"] = "1";
        }

        #region Create

        [Fact]
        public async Task CreateMotorcycle_ShouldReturnCreated_WhenServiceReturnsEntity()
        {
            // Arrange
            var req = new CreateMotorcycleRequest
            {
                Year = 2024,
                Brand = "Yamaha",
                LicensePlate = "ABC1D23"
            };

            var resp = new MotorcycleResponse
            {
                Id = 10,
                Year = 2024,
                Brand = "Yamaha",
                LicensePlate = "ABC1D23",
                CreationTime = DateTime.UtcNow
            };

            _serviceMock.Setup(s => s.CreateMotorcycle(req))
                        .ReturnsAsync(resp);

            // Act
            var result = await _controller.CreateMotorcycleAsync(req);

            // Assert
            var ok = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, ok.StatusCode);

            _serviceMock.Verify(s => s.CreateMotorcycle(req), Times.Once);
        }

        #endregion

        #region Get by Id

        [Fact]
        public async Task GetMotorcycle_ShouldReturnOk_WhenExists()
        {
            // Arrange
            var id = 5;
            var resp = new MotorcycleResponse
            {
                Id = id,
                Year = 2020,
                Brand = "Honda",
                LicensePlate = "DEF2G34",
                CreationTime = DateTime.UtcNow
            };

            _serviceMock.Setup(s => s.GetMotorcycle(id))
                        .ReturnsAsync(resp);

            // Act
            var result = await _controller.GetAsync(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var baseResponse = Assert.IsType<BaseResponse<MotorcycleResponse>>(ok.Value);
            Assert.True(baseResponse.Success);
            Assert.Equal(id, baseResponse.Data.Id);

            _serviceMock.Verify(s => s.GetMotorcycle(id), Times.Once);
        }

        #endregion

        #region Get all (with optional filter)

        [Fact]
        public async Task GetAllMotorcycles_ShouldReturnOk_WithList()
        {
            // Arrange
            var list = new List<MotorcycleResponse>
            {
                new() { Id = 1, Year = 2021, Brand = "Honda", LicensePlate = "AAA1B23", CreationTime = DateTime.UtcNow },
                new() { Id = 2, Year = 2022, Brand = "Yamaha", LicensePlate = "BBB2C34", CreationTime = DateTime.UtcNow }
            };

            _serviceMock.Setup(s => s.GetMotorcycles(null))
                        .ReturnsAsync(list);

            // Act
            var result = await _controller.GetAllAsync(null);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var baseResponse = Assert.IsType<BaseResponse<List<MotorcycleResponse>>>(ok.Value);
            Assert.True(baseResponse.Success);
            Assert.Equal(2, baseResponse.Data.Count);

            _serviceMock.Verify(s => s.GetMotorcycles(null), Times.Once);
        }

        [Fact]
        public async Task GetAllMotorcycles_WithFilter_ShouldReturnOk_FilteredList()
        {
            // Arrange
            var filter = "AAA";
            var list = new List<MotorcycleResponse>
            {
                new() { Id = 1, Year = 2021, Brand = "Honda", LicensePlate = "AAA1B23", CreationTime = DateTime.UtcNow }
            };

            _serviceMock.Setup(s => s.GetMotorcycles(filter))
                        .ReturnsAsync(list);

            // Act
            var result = await _controller.GetAllAsync(filter);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var baseResponse = Assert.IsType<BaseResponse<List<MotorcycleResponse>>>(ok.Value);
            Assert.True(baseResponse.Success);
            Assert.Single(baseResponse.Data);
            Assert.Contains("AAA", baseResponse.Data.First().LicensePlate);

            _serviceMock.Verify(s => s.GetMotorcycles(filter), Times.Once);
        }

        #endregion

        #region Update plate

        [Fact]
        public async Task UpdateLicensePlate_ShouldReturnOk_WhenUpdated()
        {
            // Arrange
            var id = 7;
            var newPlate = "JJG4393";

            _serviceMock.Setup(s => s.UpdateLicensePlate(id, newPlate))
                        .ReturnsAsync(BaseResponse<object>.Ok(null));

            // Act
            var result = await _controller.UpdateAsync(id, newPlate);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _serviceMock.Verify(s => s.UpdateLicensePlate(id, newPlate), Times.Once);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteMotorcycle_ShouldReturnOk_WhenDeleted()
        {
            // Arrange
            var id = 9;

            _serviceMock.Setup(s => s.DeleteMotorcycle(id))
                        .ReturnsAsync(BaseResponse<object>.Ok(null));

            // Act
            var result = await _controller.DeleteAsync(id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _serviceMock.Verify(s => s.DeleteMotorcycle(id), Times.Once);
        }

        #endregion
    }
}
