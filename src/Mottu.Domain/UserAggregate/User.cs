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
        PersonName name,
        DateOnly birthDate,
        CNPJ cnpj,
        CNH cnhNumber,
        ECNH cnhType,
        string? cnhImageUri
    )
    {
        var user = new User
        {
            Name = name,
            Role = EUserRole.Courier,
            BirthDate = birthDate,
            Cnpj = cnpj,
            CnhNumber = cnhNumber,
            CnhType = cnhType,
            CnhImageUri = cnhImageUri,
            CreatedAtUtc = DateTime.UtcNow
        };

        return user;
    }

    public void UpdateCnhImage(string newImageUri)
    {
        EnsureCourier();
        if (string.IsNullOrWhiteSpace(newImageUri))
            throw new BusinessRulesException("CNH image URI is required.");

        CnhImageUri = newImageUri.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateName(PersonName newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public bool IsCourierWithCategoryA()
        => Role == EUserRole.Courier
           && CnhType is not null
           && (CnhType == ECNH.A || CnhType == ECNH.AB);


    private void EnsureCourier()
    {
        if (Role != EUserRole.Courier)
            throw new BusinessRulesException("Operation allowed only for couriers.");
    }
}
