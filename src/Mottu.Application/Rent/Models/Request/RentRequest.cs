namespace Mottu.Application.Rent.Models.Request
{
    public record RentRequest
    {
        public int IdCourier { get; set; }
        public int IdMotorcycle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ForecastEndDate { get; set; }
        public int Plan { get; set; }
    }
}
