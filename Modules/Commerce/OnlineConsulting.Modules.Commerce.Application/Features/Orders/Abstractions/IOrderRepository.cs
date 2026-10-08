using System.Linq.Expressions;
using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;

public interface IOrderRepository : IAsyncRepository<Order, Guid>
{
    /// <summary>The first matching order loaded with its items, for showing it or completing it.</summary>
    Task<Order?> GetWithItemsAsync(Expression<Func<Order, bool>> predicate, bool enableTracking = true, CancellationToken cancellationToken = default);

    /// <summary>Each order's total (sum of its items), computed in the database, for list screens that don't show the items.</summary>
    Task<IReadOnlyDictionary<Guid, decimal>> GetTotalsAsync(IReadOnlyCollection<Guid> orderIds, CancellationToken cancellationToken = default);

    /// <summary>The user's order count and the total of their paid orders, computed in the database.</summary>
    Task<(int TotalOrders, decimal TotalSpent)> GetStatsAsync(Guid userId, CancellationToken cancellationToken = default);
}
