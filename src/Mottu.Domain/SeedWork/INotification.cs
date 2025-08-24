﻿using static Mottu.Domain.SeedWork.NotificationModel;

namespace Mottu.Domain.SeedWork
{
    public interface INotification
    {
        NotificationModel NotificationModel { get; }
        bool HasNotification { get; }
        void AddNotification(string key, string message, ENotificationType notificationType);
    }
}
