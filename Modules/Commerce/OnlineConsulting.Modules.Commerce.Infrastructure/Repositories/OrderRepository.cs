using System.Linq.Expressions;
using Core.PersistenceLayer.Repositories.EfRepositories;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.Modules.Commerce.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.Repositories;

public class OrderRepository(CommerceDbContext context) : EfRepositoryBase<Order, Guid, CommerceDbContext>(context), IOrderRepository
{
    public Task<Order?> GetWithItemsAsync(Expression<Func<Order, bool>> predicate, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        var query = Context.Orders.Include(o => o.Items).AsQueryable();
        if (!enableTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, decimal>> GetTotalsAsync(IReadOnlyCollection<Guid> orderIds, CancellationToken cancellationToken = default)
    {
        if (orderIds.Count == 0)
        {
            return new Dictionary<Guid, decimal>();
        }

        return await Context.Orders
            .Where(o => orderIds.Contains(o.Id))
            .Select(o => new { o.Id, Total = o.Items.Sum(i => i.TotalPrice) })
            .ToDictionaryAsync(o => o.Id, o => o.Total, cancellationToken);
    }

    public async Task<(int TotalOrders, decimal TotalSpent)> GetStatsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var totalOrders = await Context.Orders.CountAsync(o => o.UserId == userId, cancellationToken);
        var totalSpent = await Context.Orders
            .Where(o => o.UserId == userId && o.PaymentStatus == OrderPaymentStatuses.Paid)
            .SelectMany(o => o.Items)
            .SumAsync(i => i.TotalPrice, cancellationToken);

        return (totalOrders, totalSpent);
    }
}
