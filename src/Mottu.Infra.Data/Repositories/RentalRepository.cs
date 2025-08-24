using Mottu.Domain.OutboxAggregate;
using Mottu.Domain.RentalAggregate;
using Mottu.Infra.Data.Repositories.Base;

namespace Mottu.Infra.Data.Repositories
{
    public class RentalRepository(Context context) : BaseRepository<Rental>(context), IRentalRepository
    {
    }
}
