using Mottu.Domain.SeedWork;

namespace Mottu.Domain.RentalAggregate
{
    public interface IRentalRepository : IBaseRepository<Rental>, IUnitOfWork
    {
    }
}
