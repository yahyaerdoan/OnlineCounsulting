using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;

public interface IOrderRepository : IAsyncRepository<Order, Guid>
{
    /// <summary>The user's order count and the total of their paid orders, computed in the database.</summary>
    Task<(int TotalOrders, decimal TotalSpent)> GetStatsAsync(Guid userId, CancellationToken cancellationToken = default);
}
