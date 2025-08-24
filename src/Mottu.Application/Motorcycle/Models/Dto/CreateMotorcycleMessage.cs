using Mottu.Application.Motorcycle.Models.Response;

namespace Mottu.Application.Motorcycle.Models.Dto
{
    public record CreateMotorcycleMessage
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string Brand { get; set; }
        public string LicensePlate { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
