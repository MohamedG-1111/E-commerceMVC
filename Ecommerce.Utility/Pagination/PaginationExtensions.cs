using Microsoft.EntityFrameworkCore;
namespace Ecommerce.Utility.Pagination
{
    public static class PaginationExtensions
    {
        public async static Task<PaginatedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query,
            PaginationParameters parameters)
        {
            var count = query.Count();
            var items = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize)
                              .Take(parameters.PageSize)
                              .ToListAsync();

            return new PaginatedResult<T>()
            {
                Items = items,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalCount = count
            };
        }
    }

}

