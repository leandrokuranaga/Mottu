using Mottu.Domain.RentalAggregate.Enums;

namespace Fiap.Infra.Data.MapEntities.Seeds
{
    public static class RentalSeed
    {
        public static object[] Rentals() =>
        [
            new
            {
                Id = 1,
                MotorcycleId = 1,
                CourierId = 2,
                Plan = ERentalPlan.Days7,
                Status = ERentalStatus.Active,

                CreatedAtUtc = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc),

                StartDate = new DateOnly(2025, 01, 02),
                ForecastEndDate = new DateOnly(2025, 01, 08),
                EndDate = (DateOnly?)null
            }
        ];

        public static object[] RentalsDailyPrice() =>
        [
            new { RentalId = 1, Value = 30.00m, Currency = "BRL" }
        ];
    }
}
