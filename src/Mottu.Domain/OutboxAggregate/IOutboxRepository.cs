using Mottu.Domain.SeedWork;

namespace Mottu.Domain.OutboxAggregate
{
    public interface IOutboxRepository : IBaseRepository<Outbox>, IUnitOfWork
    {
    }
}
