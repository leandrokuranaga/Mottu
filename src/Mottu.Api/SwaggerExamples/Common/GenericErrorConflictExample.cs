using Mottu.Application.Common;
using Mottu.Domain.SeedWork;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Api.SwaggerExamples.Common
{
    [ExcludeFromCodeCoverage]
    public class GenericErrorConflictExample : IExamplesProvider<BaseResponse<object>>
    {
        public BaseResponse<object> GetExamples()
        {
            var notification = new NotificationModel
            {
                NotificationType = NotificationModel.ENotificationType.BusinessRules
            };

            notification.AddMessage("Field", "Field already in use");
            notification.AddMessage("Conflict", "This resource already exists and cannot be duplicated");

            return BaseResponse<object>.Fail(notification);
        }
    }

}
