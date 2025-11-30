using Cyviz.Core.Application.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cyviz.Infrastructure.Extensions
{
    public static class KeysetPaginationExtensions
    {
        public static async Task<KeysetPageResult<T>> ToKeysetPageAsync<T, TKey>(
            this IQueryable<T> query,
            Expression<Func<T, TKey>> keySelector,
            TKey? after,
            int pageSize)
        {
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 50) pageSize = 50;

            // If cursor is provided → filter
            if (after != null)
            {
                var param = keySelector.Parameters[0];
                var compare = Expression.GreaterThan(
                    keySelector.Body,
                    Expression.Constant(after));

                var lambda = Expression.Lambda<Func<T, bool>>(compare, param);
                query = query.Where(lambda);
            }

            // Sort ASC by key
            query = query.OrderBy(keySelector);

            var items = await query.Take(pageSize).ToListAsync();

            TKey? nextCursor = default;

            if (items.Count != 0)
            {
                var last = items.Last();
                nextCursor = (TKey)keySelector.Compile().Invoke(last);
            }

            return new KeysetPageResult<T>
            {
                Items = items,
                NextCursor = nextCursor?.ToString()
            };
        }
    }
}
