using Abp.Domain.Entities;
using Mottu.Domain.SeedWork.Exceptions;
using Mottu.Domain.UserAggregate.Enums;
using Mottu.Domain.UserAggregate.ValueObjects;

namespace Mottu.Domain.UserAggregate;

public sealed class User : Entity, SeedWork.IAggregateRoot
{
    public PersonName Name { get; set; } = null!;
    public EUserRole Role { get; set; }
    public DateOnly BirthDate { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public CNPJ? Cnpj { get; set; }
    public CNH? CnhNumber { get; set; } 
    public ECNH? CnhType { get; set; }
    public string? CnhImageUri { get; set; }

    public User() { }

    public static User CreateCourier(
        string name,
        DateOnly birthDate,
        string cnpj,
        string cnhNumber,
        ECNH cnhType,
        string? cnhImageUri = null
    )
    {
        return new User
        {
            Name = PersonName.Create(name),
            Role = EUserRole.Courier,
            BirthDate = birthDate,
            Cnpj = CNPJ.Create(cnpj),
            CnhNumber = CNH.Create(cnhNumber, cnhType),
            CnhType = cnhType,
            CnhImageUri = cnhImageUri,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
