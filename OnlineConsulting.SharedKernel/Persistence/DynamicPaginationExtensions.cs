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
    /// <summary>tieBreaker must be a unique column (e.g. Id) - prevents skipped/repeated rows across pages.</summary>
    public static async Task<Paginate<T>> ToDynamicPaginateAsync<T>(this IQueryable<T> query, PageRequest pageRequest, DynamicQuery? dynamicQuery, Expression<Func<T, object>> defaultOrderBy, Expression<Func<T, object>> tieBreaker, CancellationToken cancellationToken = default)
    {
        if (dynamicQuery is not null)
        {
            query = query.ToDynamic(dynamicQuery);
        }

        var ordered = dynamicQuery?.Sort is { } sort && sort.Any() ? (IOrderedQueryable<T>)query : query.OrderBy(defaultOrderBy);

        query = ordered.ThenBy(tieBreaker);

        return await query.ToPaginateAsync(pageRequest.PageIndex, pageRequest.PageSize, cancellationToken);
    }
}
