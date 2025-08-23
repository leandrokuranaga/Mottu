namespace Mottu.Application.Rent.Models.Response
{
    public record RentResponse
    {
        public int Id { get; set; }
        public decimal DailyCharge { get; set; }
        public int IdCourier { get; set; }
        public int IdMotorcycle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ForecastEndDate { get; set; }
        public DateTime ReturnDate { get; set; }


        public static explicit operator RentResponse(Domain.RentalAggregate.Rental rent) => new()
        {
            Id = rent.Id,
            DailyCharge = rent.DailyPrice.Value,
            IdCourier = rent.CourierId,
            IdMotorcycle = rent.MotorcycleId,
            StartDate = rent.StartDate.ToDateTime(new TimeOnly(0, 0)),
            EndDate = rent.EndDate?.ToDateTime(new TimeOnly(0, 0)) ?? DateTime.MinValue,
            ForecastEndDate = rent.ForecastEndDate.ToDateTime(new TimeOnly(0, 0)),
            ReturnDate = rent.EndDate?.ToDateTime(new TimeOnly(0, 0)) ?? DateTime.MinValue
        };
    }
}
