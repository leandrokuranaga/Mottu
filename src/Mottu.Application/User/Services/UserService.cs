using Microsoft.AspNetCore.Http;
using Mottu.Application.Common;
using Mottu.Application.Courier.Models.Request;
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
        public Task<BaseResponse<object>> CreateCourier(CreateCourierRequest request) => ExecuteAsync<BaseResponse<object>>(async () =>
        {
            Validate(request, new CreateCourierRequestValidator());

            var courierCNPJ = await userRepository.GetOneNoTracking(x => x.Cnpj.Number.Equals(request.CNPJ, StringComparison.CurrentCultureIgnoreCase));

            if (courierCNPJ is not null)
            {
                notification.AddNotification("Create Courier", "CNPJ already registered", NotificationModel.ENotificationType.BusinessRules);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            var courierCNH = await userRepository.GetOneNoTracking(x => x.CnhNumber.Number.Equals(request.CNH, StringComparison.CurrentCultureIgnoreCase));

            if (courierCNH is not null)
            {
                notification.AddNotification("Create Courier", "CNH already registered", NotificationModel.ENotificationType.BusinessRules);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            var courier = User.CreateCourier(request.Name, request.BirthdayDate, request.CNPJ, request.CNH, request.TypeCNH);

            await userRepository.InsertOrUpdateAsync(courier);
            await userRepository.SaveChangesAsync();

            return BaseResponse<object>.Ok(courier.Id);
        });

        public Task<BaseResponse<object>> UploadCNHPhoto(int id, IFormFile file) => ExecuteAsync<BaseResponse<object>>(async () =>
        {
            var courier = await userRepository.GetOneNoTracking(x => x.Id == id);

            if (courier is null)
            {
                notification.AddNotification("Get Courier", "Courier not found", NotificationModel.ENotificationType.NotFound);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            //jogar no outbox

            courier.CnhImageUri = $"couriers/{id}/cnh.jpg";

            await userRepository.InsertOrUpdateAsync(courier);

            objectStorage.UploadAsync(file, courier.CnhImageUri).GetAwaiter().GetResult();

            return BaseResponse<object>.Ok(null);
        });
    }
}
