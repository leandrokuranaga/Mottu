using Mottu.Domain.SeedWork;

namespace Mottu.Domain.MotorcycleAggregate
{
    public interface IMotorcycleRepository : IBaseRepository<Motorcycle>, IUnitOfWork
    {
    }
}
