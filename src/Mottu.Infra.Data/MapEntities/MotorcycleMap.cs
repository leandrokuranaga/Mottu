using Fiap.Infra.Data.MapEntities.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mottu.Domain.MotorcycleAggregate;
using System.Reflection.Emit;

namespace Mottu.Infra.Data.MapEntities
{
    internal sealed class MotorcycleMap : IEntityTypeConfiguration<Motorcycle>
    {
        public void Configure(EntityTypeBuilder<Motorcycle> b)
        {
            b.ToTable("Motorcycles");
            b.HasKey(x => x.Id);

            b.OwnsOne(x => x.Year, y =>
            {
                y.Property(p => p.Value)
                 .HasColumnName("Year")
                 .IsRequired();
            });

            b.OwnsOne(x => x.Brand, y =>
            {
                y.Property(p => p.Value)
                 .HasColumnName("Brand")
                 .HasMaxLength(60)
                 .IsRequired();
            });

            b.OwnsOne(x => x.LicensePlate, y =>
            {
                y.Property(p => p.Value)
                 .HasColumnName("LicensePlate")
                 .HasMaxLength(7)
                 .IsUnicode(false)
                 .IsRequired();

                y.HasIndex(p => p.Value).IsUnique();

                y.WithOwner();
            });

            b.Property(x => x.CreationTime).IsRequired();
            b.Property(x => x.LastModificationTime);
            b.Property(x => x.IsDeleted).IsRequired();

            b.HasQueryFilter(x => !x.IsDeleted);

            b.HasIndex(x => x.CreationTime);

            b.HasData(MotorcycleSeed.Motorcycles());

            b.OwnsOne(m => m.Year)
                .HasData(MotorcycleSeed.MotorcycleYear());

            b.OwnsOne(m => m.Brand)
                .HasData(MotorcycleSeed.MotorcycleBrand());

            b.OwnsOne(m => m.LicensePlate)
                .HasData(MotorcycleSeed.MotorcycleLicensePlate());
        }
    }
}
