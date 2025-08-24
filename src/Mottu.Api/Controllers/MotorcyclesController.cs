using Microsoft.AspNetCore.Mvc;
using Mottu.Api.SwaggerExamples.Common;
using Mottu.Application.Common;
using Mottu.Application.Motorcycle.Models.Request;
using Mottu.Application.Motorcycle.Models.Response;
using Mottu.Application.Motorcycle.Services;
using Mottu.Domain.SeedWork;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Mottu.Api.Controllers
{
    /// <summary>
    /// Controller used to manage rentals.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class MotorcyclesController(IMotorcycleService service, INotification notification) : BaseController(notification)
    {
        /// <summary>
        /// Creates a motorcycle
        /// </summary>
        /// <param name="request">The details to create a motorcycle</param>
        /// <returns>Motorcycle creation</returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a motorcycle",
            Description = "Creates a motorcycle to the platform"
        )]
        [ProducesResponseType(typeof(BaseResponse<MotorcycleResponse>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(GenericErrorBadRequestExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(GenericErrorConflictExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(GenericErrorInternalServerExample))]
        public async Task<IActionResult> CreateMotorcycleAsync([FromBody] CreateMotorcycleRequest request)
        {
            var result = await service.CreateMotorcycle(request);
            return Response<MotorcycleResponse>(0, result);
        }

        /// <summary>
        /// Gets a specific motorcycle by id
        /// </summary>
        /// <param name="id">id of the motorcycle to be consulted</param>
        /// <returns>The motorcycle information.</returns>
        [HttpGet("{id:int:min(1)}")]
        [SwaggerOperation(
            Summary = "Gets a specific motorcycle by id",
            Description = "Gets a specific motorcycle information"
        )]
        [ProducesResponseType(typeof(SuccessResponse<MotorcycleResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(GenericErrorNotFoundExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(GenericErrorInternalServerExample))]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await service.GetMotorcycle(id);
            return Response(BaseResponse<MotorcycleResponse>.Ok(result));
        }

        /// <summary>
        /// Gets all motorcycles information, with optional filtering by license plate
        /// </summary>
        /// <param name="licensePlate">Optional parameter to filter by license plate</param>
        /// <returns>The motorcycle information</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets all motorcycles information, with optional filtering by license plate",
            Description = "Gets all motorcycles information, with optional filtering by license plate"
        )]
        [ProducesResponseType(typeof(SuccessResponse<MotorcycleResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(GenericErrorBadRequestExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(GenericErrorNotFoundExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(GenericErrorInternalServerExample))]
        public async Task<IActionResult> GetAllAsync([FromQuery] string? licensePlate)
        {
            var result = await service.GetMotorcycles(licensePlate);
            return Response(BaseResponse<List<MotorcycleResponse>>.Ok(result));
        }

        /// <summary>
        /// Update license plate of a motorcycle
        /// </summary>
        /// <param name="id">The id of the motorcycle</param>
        /// <param name="licensePlate">License plate to be changed</param>
        /// <returns>The updated license plate</returns>
        [HttpPatch("{id:int:min(1)}")]
        [SwaggerOperation(
            Summary = "Update license plate of a motorcycle.",
            Description = "Update license plate of a motorcycle."
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
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] string licensePlate)
        {
            await service.UpdateLicensePlate(id, licensePlate);
            return Response(BaseResponse<object>.Ok(null));
        }

        /// <param name="id">The ID of the motorcycle to delete.</param>
        /// <returns>The deleted Motorcycle</returns>
        [HttpDelete("{id:int:min(1)}")]
        [SwaggerOperation(
            Summary = "Deletes a motorcycle by id.",
            Description = "Deletes a motorcycle by id"
        )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(GenericErrorConflictExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(GenericErrorNotFoundExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(GenericErrorInternalServerExample))]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await service.DeleteMotorcycle(id);
            return Response(BaseResponse<object>.Ok(null));
        }
    }
}
