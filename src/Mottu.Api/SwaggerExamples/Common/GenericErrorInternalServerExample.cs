using Mottu.Application.Common;
using Mottu.Domain.SeedWork;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Api.SwaggerExamples.Common
{
    [ExcludeFromCodeCoverage]
    public class GenericErrorInternalServerExample : IExamplesProvider<BaseResponse<object>>
    {
        public BaseResponse<object> GetExamples()
        {
            var notification = new NotificationModel
            {
                NotificationType = NotificationModel.ENotificationType.InternalServerError
            };

            notification.AddMessage("Server", "An unexpected error occurred while processing your request.");
            notification.AddMessage("Internal", "Please contact support if the problem persists.");

            return BaseResponse<object>.Fail(notification);
        }
    }

}
