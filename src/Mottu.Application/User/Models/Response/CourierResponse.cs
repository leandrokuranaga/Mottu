namespace Mottu.Application.User.Models.Response
{
    public record CourierResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Role { get; set; }
        public DateOnly BirthDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public string? Cnpj { get; set; }
        public string? CnhNumber { get; set; }
        public string? CnhType { get; set; }
        public string? CnhImageUri { get; set; }

        public static explicit operator CourierResponse(Mottu.Domain.UserAggregate.User user) => new()
        {
            Id = user.Id,
            Name = user.Name?.Value ?? string.Empty,
            Role = (int)user.Role,
            BirthDate = user.BirthDate,
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc,

            Cnpj = user.Cnpj?.Number,
            CnhNumber = user.CnhNumber?.Number,
            CnhType = user.CnhType?.ToString(),
            CnhImageUri = user.CnhImageUri
        };
    }        
}
