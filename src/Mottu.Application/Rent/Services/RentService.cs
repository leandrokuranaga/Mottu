using Mottu.Application.Common;
using Mottu.Application.Rent.Models.Request;
using Mottu.Application.Rent.Models.Response;
using Mottu.Application.Validators.RentValidators;
using Mottu.Domain.MotorcycleAggregate;
using Mottu.Domain.RentalAggregate;
using Mottu.Domain.SeedWork;
using Mottu.Domain.SeedWork.Exceptions;
using Mottu.Domain.UserAggregate;

namespace Mottu.Application.Rent.Services
{
    public class RentService(
        INotification notification,
        IRentalRepository rentRepository,
        IUserRepository userRepository,
        IUnitOfWork uow,
        IMotorcycleRepository motorcycleRepository
        ) : BaseService(notification), IRentService
    {
        public Task<RentResponse> GetMotorcycleRent(int id) => ExecuteAsync(async () =>
        {
            var response = new RentResponse();

            var rent = await rentRepository.GetOneNoTracking(x => x.Id == id);

            if (rent is null)
            {
                notification.AddNotification("Get Rent", "Rent not found", NotificationModel.ENotificationType.NotFound);
                return response;
            }

            response = (RentResponse)rent;

            return response;
        });

        public Task<BaseResponse<object>> RentMotorcycle(RentRequest request) => ExecuteAsync<BaseResponse<object>>(async () =>
        {

            Validate(request, new RentRequestValidator());

            var courier = await userRepository.GetOneNoTracking(x => x.Id == request.IdCourier);

            if (courier is null)
            {
                notification.AddNotification("Get Courier", "Courier not found", NotificationModel.ENotificationType.NotFound);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            var motorcycle = await motorcycleRepository.GetOneNoTracking(x => x.Id == request.IdMotorcycle);

            if (motorcycle is null)
            {
                notification.AddNotification("Get Motorcycle", "Motorcycle not found", NotificationModel.ENotificationType.NotFound);
                return BaseResponse<object>.Fail(notification.NotificationModel);
            }

            var rent = Rental.Create(
                request.IdCourier,
                request.IdMotorcycle,
                (Domain.RentalAggregate.Enums.ERentalPlan)request.Plan
            );

            await rentRepository.InsertOrUpdateAsync(rent);
            await rentRepository.SaveChangesAsync();

            return BaseResponse<object>.Ok(null);
        });

        public Task<BaseResponse<object>> ReturnMotorcycle(int id, DateTime returnDate)
           => ExecuteAsync<BaseResponse<object>>(async () =>
           {
               var rental = await rentRepository.GetByIdAsync(id, noTracking: false);
               if (rental is null)
               {
                   notification.AddNotification("Return Rental", "Rental not found", NotificationModel.ENotificationType.NotFound);
                   return BaseResponse<object>.Fail(notification.NotificationModel);
               }

               var endDate = DateOnly.FromDateTime(returnDate.Date);

                var (total, dailyBasis, feeOrExtra, isEarly, isLate) = rental.Return(endDate);

                await uow.BeginTransactionAsync();

                await rentRepository.UpdateAsync(rental);
                await rentRepository.SaveChangesAsync();

                await uow.CommitAsync();

                var payload = new
                {
                    rentalId = rental.Id,
                    plan = rental.Plan.ToString(),      
                    planDays = (int)rental.Plan,       
                    startDate = rental.StartDate,
                    forecastEndDate = rental.ForecastEndDate,
                    endDate = rental.EndDate,
                    dailyBasis = Math.Round(dailyBasis, 2),
                    feeOrExtra = Math.Round(feeOrExtra, 2),
                    total = Math.Round(total, 2),
                    isEarly,
                    isLate,
                    status = rental.Status.ToString()
                };

                    return BaseResponse<object>.Ok(payload);
           });
    }
}
