namespace Mottu.Domain.SeedWork
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        Task SaveChangesAsync();
        Task RollbackAsync();
        Task BeginTransactionAsync();
    }
}
