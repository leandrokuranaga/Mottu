using Microsoft.EntityFrameworkCore;
using Mottu.Domain.OutboxAggregate;

namespace Mottu.Infra.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Outbox> Outbox { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Context).Assembly);
        }
    }
}
