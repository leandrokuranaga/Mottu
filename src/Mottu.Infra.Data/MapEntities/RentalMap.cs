using Fiap.Infra.Data.MapEntities.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mottu.Domain.RentalAggregate;

namespace Mottu.Infra.Data.MapEntities
{
    public sealed class RentalMap : IEntityTypeConfiguration<Rental>
    {
        public void Configure(EntityTypeBuilder<Rental> b)
        {
            b.ToTable("Rentals");

            b.HasKey(x => x.Id);

            b.Property(x => x.MotorcycleId).IsRequired();
            b.Property(x => x.CourierId).IsRequired();

            b.Property(x => x.Plan).HasConversion<int>().IsRequired();
            b.Property(x => x.Status).HasConversion<int>().IsRequired();

            b.OwnsOne(x => x.DailyPrice, money =>
            {
                money.Property(m => m.Value)
                     .HasColumnName("DailyPrice")
                     .HasColumnType("decimal(18,2)")
                     .IsRequired();

                money.Property(m => m.Currency)
                     .HasColumnName("DailyPriceCurrency")
                     .HasMaxLength(3)
                     .IsUnicode(false)
                     .IsRequired();
            });

            b.OwnsOne(x => x.TotalPrice, money =>
            {
                money.Property(m => m.Value)
                     .HasColumnName("TotalPrice")
                     .HasColumnType("decimal(18,2)")
                     .IsRequired(true);

                money.Property(m => m.Currency)
                     .HasColumnName("TotalPriceCurrency")
                     .HasMaxLength(3)
                     .IsUnicode(false)
                     .IsRequired(false);
            });

            b.Property(x => x.CreatedAtUtc).IsRequired();

            b.Property(x => x.StartDate)
                .HasConversion(d => d.ToDateTime(TimeOnly.MinValue),
                               d => DateOnly.FromDateTime(d))
                .IsRequired();

            b.Property(x => x.ForecastEndDate)
                .HasConversion(d => d.ToDateTime(TimeOnly.MinValue),
                               d => DateOnly.FromDateTime(d))
                .IsRequired();

            b.Property(x => x.EndDate)
                .HasConversion(d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                               d => d.HasValue ? DateOnly.FromDateTime(d.Value) : (DateOnly?)null);

            b.HasIndex(x => x.CourierId);
            b.HasIndex(x => x.MotorcycleId);
            b.HasIndex(x => x.Status);

            b.HasData(RentalSeed.Rentals());
            b.OwnsOne(r => r.DailyPrice)
                .HasData(RentalSeed.RentalsDailyPrice());
            b.OwnsOne(r => r.TotalPrice).HasData(RentalSeed.RentalsTotalPrice());

        }
    }
}
