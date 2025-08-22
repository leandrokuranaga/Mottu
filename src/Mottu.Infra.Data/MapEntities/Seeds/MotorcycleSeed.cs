using Mottu.Domain.MotorcycleAggregate;
using Mottu.Domain.MotorcycleAggregate.ValueObjects;

namespace Fiap.Infra.Data.MapEntities.Seeds
{
    public static class MotorcycleSeed
    {
        public static List<Motorcycle> Motorcycles()
        {
            return
            [
                new Motorcycle
                {
                    Id = 1,
                    Year = ManufactureYear.Create(2020),
                    Brand = BrandName.Create("Honda"),
                    LicensePlate = LicensePlate.Create("ABC1234"),
                    CreationTime = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Motorcycle
                {
                    Id = 2,
                    Year = ManufactureYear.Create(2024),
                    Brand = BrandName.Create("Yamaha"),
                    LicensePlate = LicensePlate.Create("XYZ9A23"),
                    CreationTime = DateTime.UtcNow,
                    IsDeleted = false
                }
            ];
        }
    }
}
