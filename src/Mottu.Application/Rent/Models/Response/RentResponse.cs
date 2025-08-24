namespace Mottu.Application.Rent.Models.Response
{
    public record RentResponse
    {
        public int Id { get; set; }
        public decimal DailyCharge { get; set; }
        public int IdCourier { get; set; }
        public int IdMotorcycle { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly ForecastEndDate { get; set; }
        public DateOnly ReturnDate { get; set; }


        public static explicit operator RentResponse(Domain.RentalAggregate.Rental rent) => new()
        {
            Id = rent.Id,
            DailyCharge = rent.DailyPrice.Value,
            IdCourier = rent.CourierId,
            IdMotorcycle = rent.MotorcycleId,
            StartDate = rent.StartDate,
            EndDate = rent.EndDate,
            ForecastEndDate = rent.ForecastEndDate,
            ReturnDate = rent.EndDate ?? DateOnly.MinValue,
        };
    }
}
