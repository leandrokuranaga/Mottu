﻿using Mottu.Domain.SeedWork;
using System.Diagnostics.CodeAnalysis;

namespace Mottu.Application.Common
{
    [ExcludeFromCodeCoverage]
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public NotificationModel? Error { get; set; }

        public static BaseResponse<T> Ok(T data) =>
            new()
            { Success = true, Data = data };

        public static BaseResponse<T> Fail(NotificationModel error) =>
            new()
            { Success = false, Error = error };
    }
}
