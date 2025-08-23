using System;
namespace Fiap.Infra.Data.MapEntities.Seeds
{
    public static class MotorcycleSeed
    {
        private static DateTime Utc(int y, int m, int d, int hh = 0, int mm = 0, int ss = 0)
            => new DateTime(y, m, d, hh, mm, ss, DateTimeKind.Utc);

        public static object[] Motorcycles() =>
        [
            new
            {
                Id = 1,
                CreationTime = Utc(2025, 01, 01),
                LastModificationTime = (DateTime?)null,
                IsDeleted = false
            },
            new
            {
                Id = 2,
                CreationTime = Utc(2025, 01, 01),
                LastModificationTime = (DateTime?)null,
                IsDeleted = false
            }
        ];

        public static object[] MotorcycleYear() =>
        [
            new { MotorcycleId = 1, Value = 2020 },
            new { MotorcycleId = 2, Value = 2024 }
        ];

        public static object[] MotorcycleBrand() =>
        [
            new { MotorcycleId = 1, Value = "Honda" },
            new { MotorcycleId = 2, Value = "Yamaha" }
        ];

        public static object[] MotorcycleLicensePlate() =>
        [
            new { MotorcycleId = 1, Value = "ABC1234" },
            new { MotorcycleId = 2, Value = "XYZ9A23" }
        ];
    }
}
