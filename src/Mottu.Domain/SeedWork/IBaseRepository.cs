using System.Linq.Expressions;

namespace Mottu.Domain.SeedWork
{
    public interface IBaseRepository<TEntity>
               where TEntity : class
    {
        Task<TEntity> InsertOrUpdateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetOneNoTracking(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> GetOneTracking(Expression<Func<TEntity, bool>> expression);
        Task<IEnumerable<TEntity>> GetNoTrackingAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> GetByIdAsync(int id, bool noTracking);
        Task<IEnumerable<TEntity>> GetAllAsync();
    }
}
