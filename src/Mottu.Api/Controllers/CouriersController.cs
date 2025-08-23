using Microsoft.AspNetCore.Mvc;
using Mottu.Api.SwaggerExamples.Common;
using Mottu.Api.SwaggerExamples.Couriers;
using Mottu.Application.Common;
using Mottu.Application.Courier.Models.Request;
using Mottu.Application.Courier.Services;
using Mottu.Application.Motorcycle.Models.Response;
using Mottu.Domain.SeedWork;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Mottu.Api.Controllers
{
    /// <summary>
    /// Controller used to manage Couriers.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CouriersController(IUserService service, INotification notification) : BaseController(notification)
    {
        /// <summary>
        /// Creates a courier
        /// </summary>
        /// <param name="request">The details to create a courier</param>
        /// <returns>Courier information</returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a courier",
            Description = "Creates a courier"
        )]
        [SwaggerRequestExample(typeof(CreateCourierRequest), typeof(CreateCourierRequestExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(GenericErrorBadRequestExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(GenericErrorConflictExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(GenericErrorInternalServerExample))]
        public async Task<IActionResult> CreateCourierAsync([FromBody] CreateCourierRequest request)
        {
            var result = await service.CreateCourier(request);
            return Response<object>(BaseResponse<object>.Ok(result));
        }

        /// <summary>
        /// Sends a cnh photo for a courier
        /// </summary>
        /// <param name="id">The courier id</param>
        /// <param name="file">Photo must be a png or bmp</param>
        /// <returns>Upload photo message return</returns>
        [HttpPatch("{id:int:min(1)}")]
        [SwaggerOperation(
            Summary = "Sends a cnh photo for a courier",
            Description = "Uploads a photo for the associated courier"
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(GenericErrorBadRequestExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(GenericErrorNotFoundExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(GenericErrorConflictExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(GenericErrorInternalServerExample))]
        public async Task<IActionResult> RentMotorcycleAsync(int id, IFormFile file)
        {
            var result = await service.UploadCNHPhoto(id, file);
            return Response<object>(BaseResponse<object>.Ok(null));
        }

    }
}
