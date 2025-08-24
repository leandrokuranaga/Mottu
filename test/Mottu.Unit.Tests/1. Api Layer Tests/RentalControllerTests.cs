using Microsoft.AspNetCore.Mvc;
using Moq;
using Mottu.Api.Controllers;
using Mottu.Application.Common;
using Mottu.Application.Rent.Models.Request;
using Mottu.Application.Rent.Models.Response;
using Mottu.Application.Rent.Services;
using Mottu.Domain.SeedWork;
using System.Net;

namespace Mottu.Unit.Tests._1._Api_Layer_Tests
{
    public class RentalControllerTests
    {
        private readonly Mock<IRentService> _serviceMock;
        private readonly Mock<INotification> _notificationMock;
        private readonly RentalController _controller;

        public RentalControllerTests()
        {
            _serviceMock = new Mock<IRentService>();
            _notificationMock = new Mock<INotification>();

            _controller = new RentalController(_serviceMock.Object, _notificationMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    RouteData = new Microsoft.AspNetCore.Routing.RouteData()
                }
            };
            _controller.ControllerContext.RouteData.Values["controller"] = "Rental";
            _controller.ControllerContext.RouteData.Values["version"] = "1.0";
        }

        #region Rent (POST)

        [Fact]
        public async Task RentMotorcycle_ShouldReturnCreated_WhenServiceReturnsSuccess()
        {
            // Arrange
            var req = new RentRequest
            {
                IdCourier = 2,
                IdMotorcycle = 5,
                StartDate = new DateOnly(2025, 8, 24),
                ForecastEndDate = new DateOnly(2025, 8, 30),
                EndDate = new DateOnly(2025, 8, 30),
                Plan = 7
            };

            var resp = new RentResponse
            {
                Id = 100,
                IdCourier = 2,
                IdMotorcycle = 1,
                StartDate = req.StartDate,
                ForecastEndDate = req.ForecastEndDate,
                EndDate = null,
                DailyCharge = 0m
            };

        _serviceMock.Setup(s => s.RentMotorcycle(req))
                        .ReturnsAsync(resp);

            // Act
            var result = await _controller.RentMotorcycleAsync(req);

            // Assert
            var ok = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, ok.StatusCode);

            _serviceMock.Verify(s => s.RentMotorcycle(req), Times.Once);
        }

        #endregion

        #region Get by Id (GET)

        [Fact]
        public async Task GetRental_ShouldReturnOk_WhenExists()
        {
            // Arrange
            var id = 100;

            var resp = new RentResponse
            {
                Id = id,
                IdCourier = 2,
                IdMotorcycle = 1,
                StartDate = new DateOnly(2025, 8, 24),
                ForecastEndDate = new DateOnly(2025, 8, 30),
                EndDate = null,
                DailyCharge = 0m
            };


            _serviceMock.Setup(s => s.GetMotorcycleRent(id))
                        .ReturnsAsync(resp);

            // Act
            var result = await _controller.GetAsync(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var baseResponse = Assert.IsType<BaseResponse<RentResponse>>(ok.Value);
            Assert.True(baseResponse.Success);
            Assert.Equal(id, baseResponse.Data.Id);

            _serviceMock.Verify(s => s.GetMotorcycleRent(id), Times.Once);
        }

        #endregion

        #region Return (PATCH)

        [Fact]
        public async Task ReturnMotorcycle_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var id = 100;
            var returnDate = new DateOnly(2025, 8, 26);

            _serviceMock.Setup(s => s.ReturnMotorcycle(id, returnDate))
                        .ReturnsAsync(BaseResponse<object>.Ok(null));

            // Act
            var result = await _controller.UpdateAsync(id, returnDate);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _serviceMock.Verify(s => s.ReturnMotorcycle(id, returnDate), Times.Once);
        }

        #endregion
    }
}
