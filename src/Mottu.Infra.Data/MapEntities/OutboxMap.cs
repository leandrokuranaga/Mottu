using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mottu.Domain.OutboxAggregate;

namespace Mottu.Infra.Data.MapEntities
{
    public sealed class OutboxMap : IEntityTypeConfiguration<Outbox>
    {
        public void Configure(EntityTypeBuilder<Outbox> b)
        {
            b.ToTable("Outbox");
            b.HasKey(x => x.Id);

            b.Property(x => x.Type).IsRequired().HasMaxLength(300);
            b.Property(x => x.Content).IsRequired();

            b.Property(x => x.OccuredOn).IsRequired();
            b.Property(x => x.ProcessedOn);

            b.HasIndex(x => x.ProcessedOn);
            b.HasIndex(x => x.OccuredOn);
        }
    }
}
