using Core.PersistenceLayer.Repositories.EfRepositories;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;

public class OrderRepository(CommerceDbContext context) : EfRepositoryBase<Order, Guid, CommerceDbContext>(context), IOrderRepository
{
    public async Task<(int TotalOrders, decimal TotalSpent)> GetStatsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var totalOrders = await Context.Orders.CountAsync(o => o.UserId == userId, cancellationToken);
        var totalSpent = await Context.Orders
            .Where(o => o.UserId == userId && o.PaymentStatus == OrderPaymentStatuses.Paid)
            .Join(Context.OrderItems, o => o.Id, i => i.OrderId, (order, item) => item)
            .SumAsync(i => i.TotalPrice, cancellationToken);

        return (totalOrders, totalSpent);
    }
}
