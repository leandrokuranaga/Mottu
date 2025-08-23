using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mottu.Api.SwaggerExamples.Common;
using Mottu.Application.Common;
using Mottu.Application.Rent.Models.Request;
using Mottu.Application.Rent.Models.Response;
using Mottu.Application.Rent.Services;
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
    public class RentalController(IRentService service, INotification notification) : BaseController(notification)
    {
        /// <summary>
        /// Rents a motorcycle
        /// </summary>
        /// <param name="request">The details to rent a motorcycle</param>
        /// <returns>Rental information</returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Rents a motorcycle",
            Description = "Rents a motorcycle associated with a courier"
        )]
        [ProducesResponseType(typeof(BaseResponse<object>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(GenericErrorBadRequestExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(GenericErrorConflictExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(GenericErrorInternalServerExample))]
        public async Task<IActionResult> RentMotorcycleAsync([FromBody] RentRequest request)
        {
            var result = await service.RentMotorcycle(request);
            return Response<RentResponse>(BaseResponse<RentResponse>.Ok(null));
            //return Response<UserResponse>(result.UserId, result);
        }

        /// <summary>
        /// Consults a rental by id
        /// </summary>
        /// <param name="id">id to be consulted for the rental status.</param>
        /// <returns>A rental response</returns>
        [HttpGet("{id:int:min(1)}")]
        [SwaggerOperation(
            Summary = "Gets a rental by ID.",
            Description = "Retrieves the details of a rental by its unique identifier. Returns the rental data on success, or an error if the rental is not found or the input is invalid."
        )]
        [ProducesResponseType(typeof(SuccessResponse<RentResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(GenericErrorBadRequestExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(GenericErrorConflictExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(GenericErrorInternalServerExample))]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await service.GetMotorcycleRent(id);
            return Response(BaseResponse<RentResponse>.Ok(result));
        }

        /// <summary>
        /// Returns a rented motorcycle
        /// </summary>
        /// <param name="id">Motorcycle id to return</param>
        /// <param name="returnDate">Date of return of the motorcycle</param>
        /// <returns>A message if its succesfull or not</returns>
        [HttpPatch("{id:int:min(1)}")]
        [SwaggerOperation(
            Summary = "Creates a new regular user.",
            Description = "Registers a new user with basic access permissions. Returns the user data on success, or an error if the input is invalid or the email is already in use."
        )]
        [ProducesResponseType(typeof(SuccessResponse<RentResponse>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(GenericErrorBadRequestExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status409Conflict)]
        [SwaggerResponseExample(StatusCodes.Status409Conflict, typeof(GenericErrorConflictExample))]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(GenericErrorInternalServerExample))]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] DateTime returnDate)
        {
            var result = await service.ReturnMotorcycle(id, returnDate);
            return Response(BaseResponse<RentResponse>.Ok(null));
        }
    }
}
