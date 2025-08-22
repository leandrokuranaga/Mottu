using Mottu.Domain.UserAggregate.Enums;

namespace Fiap.Infra.Data.MapEntities.Seeds
{
    public static class UsersSeed
    {
        public static object[] Users() =>
        [
            new
            {
                Id = 1,
                Role = EUserRole.Admin,
                BirthDate = new DateOnly(1990, 01, 01),
                CreatedAtUtc = new DateTime(2025, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                UpdatedAtUtc = (DateTime?)null,
                CnhType = (ECNH?)null,
                CnhImageUri = (string?)null
            },
            new
            {
                Id = 2,
                Role = EUserRole.Courier,
                BirthDate = new DateOnly(1995, 05, 10),
                CreatedAtUtc = new DateTime(2025, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                UpdatedAtUtc = (DateTime?)null,
                CnhType = (ECNH?)ECNH.A,
                CnhImageUri = "courier1-cnh.png"
            },
            new
            {
                Id = 3,
                Role = EUserRole.Courier,
                BirthDate = new DateOnly(1998, 08, 20),
                CreatedAtUtc = new DateTime(2025, 01, 01, 00, 00, 00, DateTimeKind.Utc),
                UpdatedAtUtc = (DateTime?)null,
                CnhType = (ECNH?)ECNH.AB,
                CnhImageUri = "courier2-cnh.png"
            }
        ];

        public static object[] UsersName() =>
        [
            new { UserId = 1, Value = "Admin User" },
            new { UserId = 2, Value = "Courier One" },
            new { UserId = 3, Value = "Courier Two" }
        ];

        public static object[] UsersCnpj() =>
        [
            new { UserId = 2, Number = "12345678000195" },
            new { UserId = 3, Number = "98765432000109" }
        ];
        public static object[] UsersCnh() =>
        [
            new { UserId = 2, Number = "12345678901", Category = ECNH.A },
            new { UserId = 3, Number = "98765432100", Category = ECNH.AB }
        ];
    }
}
