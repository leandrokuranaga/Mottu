namespace Mottu.Application.Motorcycle.Models.Response
{
    public record MotorcycleResponse
    {
        public int Year { get; set; }
        public string Brand { get; set; }
        public string LicensePlate { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public bool IsDeleted { get; set; }

        public static explicit operator MotorcycleResponse(Domain.MotorcycleAggregate.Motorcycle motorcycle) =>
            new()
            {
                Year = motorcycle.Year.Value,
                Brand = motorcycle.Brand.Value,
                LicensePlate = motorcycle.LicensePlate.Value,
                CreationTime = motorcycle.CreationTime,
                LastModificationTime = motorcycle.LastModificationTime,
                IsDeleted = motorcycle.IsDeleted
            };
    }
}
