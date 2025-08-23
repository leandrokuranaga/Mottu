using Mottu.Application.Common;
using Mottu.Application.Motorcycle.Models.Request;
using Mottu.Application.Motorcycle.Models.Response;
using Mottu.Application.Validators.MotorcycleValidators;
using Mottu.Domain.MotorcycleAggregate;
using Mottu.Domain.OutboxAggregate;
using Mottu.Domain.RentalAggregate;
using Mottu.Domain.SeedWork;
using System.Text.Json;
using DomainMotorcycle = Mottu.Domain.MotorcycleAggregate.Motorcycle;



namespace Mottu.Application.Motorcycle.Services
{
    public class MotorcycleService(
        INotification notification,
        IMotorcycleRepository motorcycleRepository,
        IOutboxRepository outboxRepository,
        IRentalRepository rentalRepository,
        IUnitOfWork uow
    )
        : BaseService(notification), IMotorcycleService
    {
        public Task<MotorcycleResponse> CreateMotorcycle(CreateMotorcycleRequest request) => ExecuteAsync(async () =>
        {
            var response = new MotorcycleResponse();

            Validate(request, new CreateMotorcycleRequestValidator());

            request.LicensePlate.ToLower().Trim();

            var licensePlate = await motorcycleRepository.GetOneNoTracking(x => x.LicensePlate.Value.Equals(request.LicensePlate, StringComparison.CurrentCultureIgnoreCase));

            if (licensePlate is not null)
            {
                notification.AddNotification("Create Motorcycle", "License plate already registered", NotificationModel.ENotificationType.BusinessRules);
                return response;
            }

            DomainMotorcycle motorcycle;

            motorcycle = DomainMotorcycle.Create(request.Year, request.Brand, request.LicensePlate);

            //converter para objeto outbox

            Outbox outbox = Outbox.Create(
                "MotorcycleCreated",
                JsonSerializer.Serialize(new { motorcycle.Id, motorcycle.LicensePlate, motorcycle.Brand, motorcycle.Year })
            );

            await uow.BeginTransactionAsync();

            await outboxRepository.InsertOrUpdateAsync(outbox);
            await outboxRepository.SaveChangesAsync();

            await motorcycleRepository.InsertOrUpdateAsync(motorcycle);
            await motorcycleRepository.SaveChangesAsync();

            await uow.CommitAsync();

            return (MotorcycleResponse)motorcycle;
        });

        public Task<BaseResponse<object>> DeleteMotorcycle(int id) => ExecuteAsync<BaseResponse<object>>(async () =>
        {
            var motorcycle = await motorcycleRepository.GetOneNoTracking(x => x.Id == id);

            if (motorcycle is null)
            {
                notification.AddNotification("Get Motorcycle", "Motorcycle not found", NotificationModel.ENotificationType.NotFound);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            var entries = await rentalRepository.GetNoTrackingAsync(x => x.MotorcycleId == id);

            if (entries is not null)
            {
                notification.AddNotification("Delete Motorcycle", "Motorcycle has rental entries and cannot be deleted", NotificationModel.ENotificationType.BusinessRules);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            await motorcycleRepository.DeleteAsync(motorcycle);
            await motorcycleRepository.SaveChangesAsync();

            return BaseResponse<object>.Ok(null);
        });

        public Task<MotorcycleResponse> GetMotorcycle(int id) => ExecuteAsync(async () =>
        {
            var response = new MotorcycleResponse();

            var motorcycle = await motorcycleRepository.GetOneNoTracking(x => x.Id == id);

            if (motorcycle is null)
            {
                notification.AddNotification("Get Motorcycle", "Motorcycle not found", NotificationModel.ENotificationType.NotFound);
                return response;
            }

            // converter moto

            return response;
        });

        public Task<List<MotorcycleResponse>> GetMotorcycles(string? licensePlate) => ExecuteAsync(async () =>
        {
            var response = new List<MotorcycleResponse>();

            if (!string.IsNullOrWhiteSpace(licensePlate))
            {
                licensePlate = licensePlate.ToLower().Trim();

                var motorcyclesWithLicense = await motorcycleRepository.GetNoTrackingAsync(x => x.LicensePlate.Value.Contains(licensePlate, StringComparison.CurrentCultureIgnoreCase));

                if (motorcyclesWithLicense is null)
                {
                    notification.AddNotification("Get Motorcycle", "License plate doesnt exists", NotificationModel.ENotificationType.NotFound);
                    return response;
                }
            }

            var motorcycles = await motorcycleRepository.GetAllAsync();

            return response;
        });

        public Task<BaseResponse<object>> UpdateLicensePlate(int id, string newLicensePlate) => ExecuteAsync<BaseResponse<object>>(async () =>
        {
            var updateMotorcycle = await motorcycleRepository.GetOneNoTracking(x => x.Id == id);

            if (updateMotorcycle is null)
            {
                notification.AddNotification("Update Motorcycle", "Motorcycle not found", NotificationModel.ENotificationType.NotFound);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            updateMotorcycle.ChangePlate(newLicensePlate);

            await motorcycleRepository.SaveChangesAsync();

            return BaseResponse<object>.Ok(null);
        });
    }
}
