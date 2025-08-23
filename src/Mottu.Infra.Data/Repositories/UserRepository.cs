using Mottu.Domain.UserAggregate;
using Mottu.Infra.Data.Repositories.Base;

namespace Mottu.Infra.Data.Repositories
{
    public class UserRepository(Context context) : BaseRepository<User>(context), IUserRepository
    {
    }
}
