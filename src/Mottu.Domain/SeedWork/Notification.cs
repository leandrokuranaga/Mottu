using System.Diagnostics.CodeAnalysis;
using static Mottu.Domain.SeedWork.NotificationModel;

namespace Mottu.Domain.SeedWork
{
    [ExcludeFromCodeCoverage]
    public class Notification : INotification
    {
        private readonly NotificationModel _notification = new();

        public bool HasNotification => _notification.FieldMessages.Any() || _notification.GeneralMessages.Any();
        public NotificationModel NotificationModel => _notification;

        public void AddNotification(string key, string message, ENotificationType type)
        {
            _notification.NotificationType = type;

            var msg = new NotificationModel.NotificationMessage
            {
                Key = key,
                Message = message
            };

            if (type == ENotificationType.BadRequestError)
            {
                _notification.FieldMessages.Add(msg);
            }
            else
            {
                _notification.GeneralMessages.Add(msg);
            }
        }
    }
}
