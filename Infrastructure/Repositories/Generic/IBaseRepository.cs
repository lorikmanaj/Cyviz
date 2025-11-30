using System.Linq.Expressions;

namespace Cyviz.Infrastructure.Repositories.Generic
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);

        IQueryable<T> Query();

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);
        void Remove(T entity);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        Task<int> SaveChangesAsync();
    }
}
