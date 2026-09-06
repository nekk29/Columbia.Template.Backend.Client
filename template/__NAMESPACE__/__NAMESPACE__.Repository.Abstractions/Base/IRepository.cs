using System.Linq.Expressions;
using __NAMESPACE__.Entity.Base;

namespace __NAMESPACE__.Repository.Abstractions.Base
{
    public interface IRepository<TEntity>
    {
        Task<TEntity?> AddAsync(TEntity entity);
        Task<IEnumerable<TEntity>?> AddAsync(params TEntity[] entities);
        Task<TEntity?> UpdateAsync(TEntity entity);
        Task<IEnumerable<TEntity>?> UpdateAsync(params TEntity[] entities);
        Task<int> DeleteAsync(TEntity entity);
        Task<int> DeleteAsync(params TEntity[] entities);
        Task<TEntity?> GetAsync(object keyValue);
        Task<TEntity?> GetAsNoTrackingAsync(object keyValue);
        Task<TEntity?> GetAsync(params object[] keyValues);
        Task<TEntity?> GetAsNoTrackingAsync(params object[] keyValues);
        Task<TEntity?> GetAsync(object keyValue, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity?> GetAsNoTrackingAsync(object keyValue, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity?> GetAsync(object[] keyValues, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity?> GetAsNoTrackingAsync(object[] keyValues, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity?> GetByAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity?> GetByAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> FindAll();
        IQueryable<TEntity> FindAllAsNoTracking();
        IQueryable<TEntity> FindAll(params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> FindAllAsNoTracking(params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> filter);
        Task<IEnumerable<TEntity>> FindByAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter);
        Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IEnumerable<TEntity>> FindByAsNoTrackingAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<SearchResult<TEntity>> SearchByAsync(int page, int pageSize, IEnumerable<SortExpression<TEntity>>? sortExpressions, Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<SearchResult<TEntity>> SearchByAsNoTrackingAsync(int page, int pageSize, IEnumerable<SortExpression<TEntity>>? sortExpressions, Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<int> SaveAsync();
        void UpdateAuditTrails(TEntity entity, bool creation = true);
    }
}
