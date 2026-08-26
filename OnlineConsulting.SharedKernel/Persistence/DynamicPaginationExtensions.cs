using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Dynamics.Extensions;
using Core.PersistenceLayer.Pagings.Extensions;
using Core.PersistenceLayer.Pagings.Paging;
using System.Linq.Expressions;

namespace OnlineConsulting.SharedKernel.Persistence;

/// <summary>Filter + sort + page in one call. Apply tenant/ownership .Where() before this - not included.</summary>
public static class DynamicPaginationExtensions
{
    public static async Task<Paginate<T>> ToDynamicPaginateAsync<T>(
        this IQueryable<T> query, PageRequest pageRequest, DynamicQuery? dynamicQuery,
        Expression<Func<T, object>> defaultOrderBy, CancellationToken cancellationToken = default)
    {
        if (dynamicQuery is not null)
        {
            query = query.ToDynamic(dynamicQuery);
        }

        if (dynamicQuery?.Sort is not { } sort || !sort.Any())
        {
            query = query.OrderBy(defaultOrderBy);
        }

        return await query.ToPaginateAsync(pageRequest.PageIndex, pageRequest.PageSize, cancellationToken);
    }
}
