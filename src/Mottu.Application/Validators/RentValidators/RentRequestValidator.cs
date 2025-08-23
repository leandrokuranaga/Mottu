using FluentValidation;
using Mottu.Application.Rent.Models.Request;
using Mottu.Domain.RentalAggregate.Enums;
using Mottu.Domain.RentalAggregate.Services;

namespace Mottu.Application.Validators.RentValidators
{
    public class RentRequestValidator : AbstractValidator<RentRequest>
    {
        public RentRequestValidator()
        {
            RuleFor(x => x.IdCourier)
                .GreaterThan(0).WithMessage("IdCourier must be greater than 0.");

            RuleFor(x => x.IdMotorcycle)
                .GreaterThan(0).WithMessage("IdMotorcycle must be greater than 0.");

            RuleFor(x => x.Plan)
                .Must(IsValidPlan)
                .WithMessage("Plan must be one of: 7, 15, 30, 45, 50.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.")
                .Must(IsTomorrowOrLater)
                .WithMessage(_ => $"StartDate must be tomorrow or later (UTC).");

            When(x => IsValidPlan(x.Plan), () =>
            {
                RuleFor(x => x.ForecastEndDate)
                    .NotEmpty().WithMessage("ForecastEndDate is required.")
                    .Must((req, forecast) =>
                    {
                        var (_, _, days) = RentalPlanCatalog.Get((ERentalPlan)req.Plan);
                        var expected = req.StartDate.Date.AddDays(days - 1);
                        return forecast.Date == expected;
                    })
                    .WithMessage(req =>
                    {
                        var (_, _, days) = RentalPlanCatalog.Get((ERentalPlan)req.Plan);
                        var expected = req.StartDate.Date.AddDays(days - 1);
                        return $"ForecastEndDate must be StartDate + {days - 1} days (expected {expected:yyyy-MM-dd}).";
                    });
            });

            When(x => x.EndDate != default, () =>
            {
                RuleFor(x => x.EndDate)
                    .Must((req, end) => end.Date >= req.StartDate.Date)
                    .WithMessage("EndDate cannot be before StartDate.");
            });
        }

        private static bool IsValidPlan(int plan)
            => Enum.IsDefined(typeof(ERentalPlan), plan);

        private static bool IsTomorrowOrLater(DateTime startUtc)
        {
            var todayUtc = DateTime.UtcNow.Date;
            return startUtc.Date >= todayUtc.AddDays(1);
        }
    }
}
