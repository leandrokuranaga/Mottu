using Abp.Domain.Entities;
using Mottu.Domain.MotorcycleAggregate.ValueObjects;
using Mottu.Domain.SeedWork.Exceptions;

namespace Mottu.Domain.MotorcycleAggregate;

public sealed class Motorcycle
    : Entity, SeedWork.IAggregateRoot, ISoftDelete
{
    public ManufactureYear Year { get; set; } = null!;
    public BrandName Brand { get; set; } = null!;
    public LicensePlate LicensePlate { get; set; } = null!;

    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public bool IsDeleted { get; set; }

    public Motorcycle() { }

    public static Motorcycle Create(int year, string brand, string plate)
    {
        var moto = new Motorcycle
        {
            Year = ManufactureYear.Create(year),
            Brand = BrandName.Create(brand),
            LicensePlate = LicensePlate.Create(plate),
            CreationTime = DateTime.UtcNow
        };

        return moto;
    }

    public void ChangePlate(string newPlate)
    {
        EnsureNotDeleted();
        var newVo = LicensePlate.Create(newPlate);
        if (newVo.Value == LicensePlate.Value) return;

        LicensePlate = newVo;
        LastModificationTime = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        EnsureNotDeleted();
        IsDeleted = true;
        LastModificationTime = DateTime.UtcNow;
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted) throw new BusinessRulesException("Motorcycle already deleted.");
    }
}
