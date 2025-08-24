using Microsoft.AspNetCore.Mvc;
using Mottu.Application.Common;
using Mottu.Domain.SeedWork;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Mottu.Api
{
    [ExcludeFromCodeCoverage]
    public class BaseController(INotification notification) : ControllerBase
    {
        private bool IsValidOperation() => !notification.HasNotification;

        protected IActionResult Response<T>(BaseResponse<T> response)
        {
            if (IsValidOperation())
            {
                if (response.Data == null)
                    return NoContent();

                return Ok(response);
            }

            response.Success = false;
            response.Data = default;
            response.Error = notification.NotificationModel;

            return response.Error.NotificationType switch
            {
                NotificationModel.ENotificationType.BusinessRules => Conflict(response),
                NotificationModel.ENotificationType.NotFound => NotFound(response),
                NotificationModel.ENotificationType.BadRequestError => BadRequest(response),
                _ => StatusCode((int)HttpStatusCode.InternalServerError, response)
            };
        }

        protected new IActionResult Response<T>(int? id, object response)
        {
            if (!IsValidOperation())
            {
                var statusCode = MapNotificationToStatusCode(notification.NotificationModel.NotificationType);

                return StatusCode(statusCode, new
                {
                    success = false,
                    error = notification.NotificationModel
                });
            }

            if (id == null)
                return Ok(new { success = true, data = response });

            var controller = ControllerContext.RouteData.Values["controller"]?.ToString();
            var version = RouteData.Values["version"]?.ToString();
            var location = $"/api/v{version}/{controller}/{id}";

            return Created(location, new { success = true, data = response ?? new object() });
        }

        private int MapNotificationToStatusCode(NotificationModel.ENotificationType notificationType)
        {
            return notificationType switch
            {
                NotificationModel.ENotificationType.InternalServerError => StatusCodes.Status500InternalServerError,
                NotificationModel.ENotificationType.BusinessRules => StatusCodes.Status409Conflict,
                NotificationModel.ENotificationType.NotFound => StatusCodes.Status404NotFound,
                NotificationModel.ENotificationType.BadRequestError => StatusCodes.Status400BadRequest,
                NotificationModel.ENotificationType.Default => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };
        }
    }
}
