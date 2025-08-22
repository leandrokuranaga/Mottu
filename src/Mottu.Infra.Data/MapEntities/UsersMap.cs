using Fiap.Infra.Data.MapEntities.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mottu.Domain.UserAggregate;

namespace Mottu.Infra.Data.MapEntities
{
    public sealed class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.ToTable("Users");

            b.HasKey(x => x.Id);

            b.Property(x => x.Role)
                .HasConversion<int>()
                .IsRequired();

            b.Property(x => x.CreatedAtUtc).IsRequired();
            b.Property(x => x.UpdatedAtUtc);

            b.Property(x => x.BirthDate)
                .HasConversion(
                    d => d.ToDateTime(TimeOnly.MinValue),
                    d => DateOnly.FromDateTime(d))
                .IsRequired();

            b.OwnsOne(x => x.Name, name =>
            {
                name.Property(p => p.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(120)
                    .IsRequired();

                name.HasIndex(p => p.Value);
            });

            b.OwnsOne(x => x.Cnpj, cnpj =>
            {
                cnpj.Property(p => p.Number)
                    .HasColumnName("Cnpj")
                    .HasMaxLength(14)
                    .IsUnicode(false);

                cnpj.HasIndex(p => p.Number).IsUnique();
            });

            b.OwnsOne(x => x.CnhNumber, cnh =>
            {
                cnh.Property(p => p.Number)
                    .HasColumnName("CnhNumber")
                    .HasMaxLength(11)
                    .IsUnicode(false);

                cnh.Property(p => p.Category)
                    .HasColumnName("CnhCategory")
                    .HasConversion<int>();

                cnh.HasIndex(p => p.Number).IsUnique();
            });

            b.Property(x => x.CnhType).HasConversion<int?>();
            b.Property(x => x.CnhImageUri).HasMaxLength(500);

            b.HasIndex(x => x.Role);
            b.HasIndex(x => x.BirthDate);

            b.HasData(UsersSeed.Users());

            b.OwnsOne(x => x.Name).HasData(UsersSeed.UsersName());

            b.OwnsOne(x => x.Cnpj).HasData(UsersSeed.UsersCnpj());

            b.OwnsOne(x => x.CnhNumber).HasData(UsersSeed.UsersCnh());
        }
    }
}
