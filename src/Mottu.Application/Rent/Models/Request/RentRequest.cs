namespace Mottu.Application.Rent.Models.Request
{
    public record RentRequest
    {
        public int IdCourier { get; set; }
        public int IdMotorcycle { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateOnly ForecastEndDate { get; set; }
        public int Plan { get; set; }
    }
}
