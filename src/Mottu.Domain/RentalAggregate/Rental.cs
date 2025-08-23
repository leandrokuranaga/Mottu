using Abp.Domain.Entities;
using Mottu.Domain.RentalAggregate.Enums;
using Mottu.Domain.RentalAggregate.Services;
using Mottu.Domain.RentalAggregate.ValueObjects;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Domain.RentalAggregate;

public sealed class Rental : Entity, SeedWork.IAggregateRoot
{
    public int MotorcycleId { get; set; }
    public int CourierId { get; set; }
    public ERentalPlan Plan { get; set; }
    public Money DailyPrice { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly ForecastEndDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public ERentalStatus Status { get; set; }

    public Rental() { }

    public static Rental Create(int motorcycleId, int courierId, ERentalPlan plan)
    {
        var (daily, earlyPct, days) = RentalPlanCatalog.Get(plan);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var start = today.AddDays(1);
        var forecastEnd = start.AddDays(days - 1);

        var rental = new Rental
        {
            MotorcycleId = motorcycleId,
            CourierId = courierId,
            Plan = plan,
            DailyPrice = new Money(daily),
            CreatedAtUtc = DateTime.UtcNow,
            StartDate = start,
            ForecastEndDate = forecastEnd,
            Status = ERentalStatus.Pending
        };

        return rental;
    }

    public void ActivateIfStartsToday(DateOnly today)
    {
        if (Status == ERentalStatus.Pending && StartDate <= today && EndDate is null)
        {
            Status = ERentalStatus.Active;
        }
    }

    public (decimal total, decimal dailyBasis, decimal feeOrExtra, bool isEarly, bool isLate) Return(DateOnly endDate)
    {
        if (Status is ERentalStatus.Closed or ERentalStatus.Canceled)
            throw new BusinessRulesException("Rental already closed.");

        if (endDate < StartDate)
            throw new BusinessRulesException("End date cannot be before start date.");

        EndDate = endDate;

        var daysUsed = (endDate.ToDateTime(TimeOnly.MinValue) - StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;
        var plannedDays = (ForecastEndDate.ToDateTime(TimeOnly.MinValue) - StartDate.ToDateTime(TimeOnly.MinValue)).Days + 1;

        var dailyBasis = daysUsed * DailyPrice.Value;

        bool isEarly = endDate < ForecastEndDate;
        bool isLate = endDate > ForecastEndDate;

        decimal feeOrExtra = 0m;

        if (isEarly)
        {
            var (_, earlyPct, _) = RentalPlanCatalog.Get(Plan);
            if (earlyPct is not null)
            {
                var notUsed = plannedDays - daysUsed;
                feeOrExtra = notUsed * DailyPrice.Value * earlyPct.Value;
            }
        }
        else if (isLate)
        {
            var aditional = (endDate.ToDateTime(TimeOnly.MinValue) - ForecastEndDate.ToDateTime(TimeOnly.MinValue)).Days;
            feeOrExtra = aditional * 50m;
        }

        var total = dailyBasis + feeOrExtra;
        Status = ERentalStatus.Closed;

        return (total, dailyBasis, feeOrExtra, isEarly, isLate);
    }

}
