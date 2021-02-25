using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NeinLinq;

namespace RestApiDemo.WebApi.Pagination
{
    public static class PagingExtensions
    {
        /// <summary>
        /// Create a paging list based on the EntityFramework query with a given sort order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">Entity Framework query</param>
        /// <param name="offset">The offset of the first record to return</param>
        /// <param name="limit">The number of records to return</param>
        /// <param name="sortExpression">Sort expression</param>
        /// <param name="defaultSortExpression">Default sort expression</param>
        /// <returns>The PagingListOfT</returns>
        public static async Task<PagingList<T>> ToPagingListAsync<T>(this IQueryable<T> query,
            int offset, int limit,
            string sortExpression, string defaultSortExpression) where T : class
        {
            var totalRecordCount = await query.CountAsync();

            var sort = string.IsNullOrEmpty(sortExpression)
                ? defaultSortExpression
                : sortExpression;

            if (sort.StartsWith("-"))
            {
                query = query.OrderBy(sort.Substring(1), @descending: true);
            }
            else
            {
                query = query.OrderBy(sort);
            }

            var data = await query.Skip(offset).Take(limit).ToListAsync().ConfigureAwait(false);
            return new PagingList<T>(data, totalRecordCount);
        }
    }
}