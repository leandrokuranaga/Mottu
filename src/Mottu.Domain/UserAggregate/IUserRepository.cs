using Mottu.Domain.SeedWork;

namespace Mottu.Domain.UserAggregate
{
    public interface IUserRepository : IBaseRepository<User>, IUnitOfWork
    {
    }
}
