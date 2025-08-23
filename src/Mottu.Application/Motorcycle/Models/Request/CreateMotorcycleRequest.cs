namespace Mottu.Application.Motorcycle.Models.Request
{
    public record CreateMotorcycleRequest
    {
        public int Year { get; set; }
        public string Brand { get; set; }
        public string LicensePlate { get; set; }
    }
}
