using Microsoft.AspNetCore.Http;
using Mottu.Application.Common;
using Mottu.Application.Courier.Models.Request;
using Mottu.Application.User.Models.Response;
using Mottu.Application.Validators.CourierValidators;
using Mottu.Domain.SeedWork;
using Mottu.Domain.UserAggregate;
using Mottu.Infra.Storage;

namespace Mottu.Application.Courier.Services
{
    public class UserService(
        INotification notification,
        IUserRepository userRepository,
        IUnitOfWork uow,
        IObjectStorage objectStorage
    )
        : BaseService(notification), IUserService
    {
        public Task<CourierResponse> CreateCourier(CreateCourierRequest request) => ExecuteAsync(async () =>
        {
            var response = new CourierResponse();

            Validate(request, new CreateCourierRequestValidator());

            var courierCNPJ = await userRepository.GetOneNoTracking(x => x.Cnpj.Number == request.CNPJ);

            if (courierCNPJ is not null)
            {
                notification.AddNotification("Create Courier", "CNPJ already registered", NotificationModel.ENotificationType.BusinessRules);
                return response;
            }

            var courierCNH = await userRepository.GetOneNoTracking(x => x.CnhNumber.Number == request.CNH);

            if (courierCNH is not null)
            {
                notification.AddNotification("Create Courier", "CNH already registered", NotificationModel.ENotificationType.BusinessRules);
                return response;
            }

            var courier = Mottu.Domain.UserAggregate.User.CreateCourier(request.Name, request.BirthdayDate, request.CNPJ, request.CNH, request.TypeCNH);

            await userRepository.InsertOrUpdateAsync(courier);
            await userRepository.SaveChangesAsync();

            response = (CourierResponse)courier;

            return response;
        });

        public Task<CourierResponse> GetCourier(int id)
            => ExecuteAsync(async () =>
        {
            var response = new CourierResponse();

            var courier = await userRepository.GetOneNoTracking(x => x.Id == id);

            if (courier is null)
            {
                notification.AddNotification("Get Courier", "Courier not found", NotificationModel.ENotificationType.NotFound);
                return response;
            }

            response = (CourierResponse)courier;

            return response;
        });

        public Task<BaseResponse<object>> UploadCNHPhoto(int id, IFormFile file) => ExecuteAsync(async () =>
        {
            if (file is null || file.Length == 0)
            {
                notification.AddNotification("Upload CNH", "File is empty", NotificationModel.ENotificationType.BusinessRules);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            var validContentTypes = new[] { "image/png", "image/bmp" };
            if (!validContentTypes.Contains(file.ContentType.ToLower()))
            {
                notification.AddNotification("Upload CNH", "Only bmp or png are accepted", NotificationModel.ENotificationType.BusinessRules);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".png" && ext != ".bmp")
            {
                notification.AddNotification("Upload CNH", "Only bmp or png are accepted", NotificationModel.ENotificationType.BusinessRules);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }
            var courier = await userRepository.GetOneTracking(x => x.Id == id);

            if (courier is null)
            {
                notification.AddNotification("Get Courier", "Courier not found", NotificationModel.ENotificationType.NotFound);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            courier.CnhImageUri = $"couriers/{id}/cnh.jpg";

            await userRepository.InsertOrUpdateAsync(courier);

            objectStorage.UploadAsync(file, courier.CnhImageUri).GetAwaiter().GetResult();

            await userRepository.SaveChangesAsync();

            return BaseResponse<object>.Ok(null);
        });
    }
}
