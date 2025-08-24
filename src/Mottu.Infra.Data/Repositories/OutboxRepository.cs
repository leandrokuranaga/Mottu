using Mottu.Domain.OutboxAggregate;
using Mottu.Infra.Data.Repositories.Base;

namespace Mottu.Infra.Data.Repositories
{
    public class OutboxRepository(Context context) : BaseRepository<Outbox>(context), IOutboxRepository
    {
    }
}
