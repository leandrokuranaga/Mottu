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

        public async Task<RentResponse> RentMotorcycle(RentRequest request) 
            //=> ExecuteAsync(async () =>
        {
            var response = new RentResponse();

            Validate(request, new RentRequestValidator());

            var courier = await userRepository.GetOneNoTracking(x => x.Id == request.IdCourier);

            if (courier is null)
            {
                notification.AddNotification("Get Courier", "Courier not found", NotificationModel.ENotificationType.NotFound);
                return response;
            }

            var motorcycle = await motorcycleRepository.GetOneNoTracking(x => x.Id == request.IdMotorcycle);

            if (motorcycle is null)
            {
                notification.AddNotification("Get Motorcycle", "Motorcycle not found", NotificationModel.ENotificationType.NotFound);
                return response;
            }

            var isAvailable = await rentRepository.GetOneNoTracking(x 
                => x.MotorcycleId == request.IdMotorcycle 
                && x.Status == Domain.RentalAggregate.Enums.ERentalStatus.Active
                || x.Status == Domain.RentalAggregate.Enums.ERentalStatus.Pending
                || x.CourierId == request.IdCourier
                );

            if (isAvailable is not null)
            {
                notification.AddNotification("Rent Motorcycle", "Motorcycle not available to rent", NotificationModel.ENotificationType.BusinessRules);
                return response;
            }

            var rent = Rental.Create(
                request.IdCourier,
                request.IdMotorcycle,
                (Domain.RentalAggregate.Enums.ERentalPlan)request.Plan
            );

            await rentRepository.InsertOrUpdateAsync(rent);
            await rentRepository.SaveChangesAsync();

            response = (RentResponse)rent;

            return response;
        }
        //);

        public Task<BaseResponse<object>> ReturnMotorcycle(int id, DateOnly returnDate)
           => ExecuteAsync(async () =>
           {
               var rental = await rentRepository.GetByIdAsync(id, noTracking: false);
               if (rental is null)
               {
                   notification.AddNotification("Return Rental", "Rental not found", NotificationModel.ENotificationType.NotFound);
                   return BaseResponse<object>.Fail(notification.NotificationModel);
               }

                var (total, dailyBasis, feeOrExtra, isEarly, isLate) = rental.Return(returnDate);

                await rentRepository.UpdateAsync(rental);
                await rentRepository.SaveChangesAsync();

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
