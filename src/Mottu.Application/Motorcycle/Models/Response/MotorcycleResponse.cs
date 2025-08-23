namespace Mottu.Application.Motorcycle.Models.Response
{
    public record MotorcycleResponse
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Brand { get; set; }
        public string LicensePlate { get; set; }

        public static explicit operator MotorcycleResponse(Domain.MotorcycleAggregate.Motorcycle motorcycle) =>
            new()
            {
                Id = motorcycle.Id,
                Year = motorcycle.Year.Value,
                Brand = motorcycle.Brand.Value,
                LicensePlate = motorcycle.LicensePlate.Value
            };
    }
}
