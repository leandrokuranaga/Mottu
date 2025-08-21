using Mottu.Application.Common;
using Mottu.Domain.SeedWork;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Api.SwaggerExamples.Common
{
    [ExcludeFromCodeCoverage]
    public class GenericErrorBadRequestExample : IExamplesProvider<BaseResponse<object>>
    {
        public BaseResponse<object> GetExamples()
        {
            var notification = new NotificationModel
            {
                NotificationType = NotificationModel.ENotificationType.BadRequestError
            };

            notification.AddMessage("Field", "Field Required");

            notification.AddMessage("Error", "Generic validation error");

            return BaseResponse<object>.Fail(notification);
        }
    }

}
