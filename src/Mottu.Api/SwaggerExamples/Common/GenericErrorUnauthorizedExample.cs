﻿using Mottu.Application.Common;
using Mottu.Domain.SeedWork;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Api.SwaggerExamples.Common
{
    [ExcludeFromCodeCoverage]
    public class GenericErrorUnauthorizedExample : IExamplesProvider<BaseResponse<object>>
    {
        public BaseResponse<object> GetExamples()
        {
            var notification = new NotificationModel
            {
                NotificationType = NotificationModel.ENotificationType.Unauthorized
            };

            notification.AddMessage("Authorization", "You are not authorized to perform this action.");
            notification.AddMessage("Token", "Missing or invalid authentication token.");

            return BaseResponse<object>.Fail(notification);
        }
    }

}
