using Mottu.Domain.RentalAggregate.Enums;

namespace Mottu.Domain.RentalAggregate.Services
{
    public static class RentalPlanCatalog
    {
        public static (decimal daily, decimal? earlyPenaltyPct, int days) Get(ERentalPlan plan) => plan switch
        {
            ERentalPlan.Days7 => (30m, 0.20m, 7),
            ERentalPlan.Days15 => (28m, 0.40m, 15),
            ERentalPlan.Days30 => (22m, null, 30),
            ERentalPlan.Days45 => (20m, null, 45),
            ERentalPlan.Days50 => (18m, null, 50),
            _ => throw new ArgumentOutOfRangeException(nameof(plan))
        };
    }
}
