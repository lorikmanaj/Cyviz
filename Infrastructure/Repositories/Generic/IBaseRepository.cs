using System.Linq.Expressions;

namespace Cyviz.Infrastructure.Repositories.Generic
{
    public interface IBaseRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task SaveChangesAsync();

        Task<T?> GetByIdAsync(string id);
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        Task DeleteAsync(string id);
        Task DeleteAsync(Guid id);
        Task DeleteAsync(int id);
    }
}
