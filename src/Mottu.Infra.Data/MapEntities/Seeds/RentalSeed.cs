using Mottu.Domain.RentalAggregate.Enums;

public static class RentalSeed
{
    private static DateTime Utc(int y, int m, int d, int hh = 0, int mm = 0, int ss = 0)
        => new(y, m, d, hh, mm, ss, DateTimeKind.Utc);

    public static object[] Rentals() =>
    [
        new
        {
            Id = 1,
            MotorcycleId = 1,
            CourierId = 2,
            Plan = ERentalPlan.Days7,
            Status = ERentalStatus.Active,
            CreatedAtUtc = Utc(2025, 01, 01),
            StartDate = new DateOnly(2025, 01, 02),
            ForecastEndDate = new DateOnly(2025, 01, 09),
            EndDate = (DateOnly?)null
        }
    ];

    public static object[] RentalsDailyPrice() =>
    [
        new { RentalId = 1, Value = 30.00m, Currency = "BRL" }
    ];

    public static object[] RentalsTotalPrice() =>
    [
        new { RentalId = 1, Value = 150.00m, Currency = "BRL" }
    ];
}
