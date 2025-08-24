using Mottu.Domain.MotorcycleAggregate;
using Mottu.Infra.Data.Repositories.Base;

namespace Mottu.Infra.Data.Repositories
{
    public class MotorcycleRepository(Context context) : BaseRepository<Motorcycle>(context), IMotorcycleRepository
    {
    }
}
