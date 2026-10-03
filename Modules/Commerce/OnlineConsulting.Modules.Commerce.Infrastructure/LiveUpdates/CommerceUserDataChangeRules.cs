using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Commerce.Domain;
using OnlineConsulting.SharedKernel.LiveUpdates;

namespace OnlineConsulting.Modules.Commerce.Infrastructure.LiveUpdates;

/// <summary>Basket, order and address rows mapped to the owning user's live-update topics. Item rows only know their
/// parent id, so the parent is looked up - first among rows saved in the same call (a new basket with its first item), then in the database.</summary>
public static class CommerceUserDataChangeRules
{
    public static void Configure(UserDataChangeRuleSet rules) => rules
        .ForUserProperties<Basket>(UserDataTopics.Basket, nameof(Basket.UserId))
        .For<BasketItem>(UserDataTopics.Basket, async (entry, cancellationToken) =>
            await BasketOwnerAsync(entry.Context, entry.Entity.BasketId, cancellationToken) is Guid userId ? [userId] : [])
        .ForUserProperties<Order>(UserDataTopics.Orders, nameof(Order.UserId))
        .For<OrderItem>(UserDataTopics.Orders, async (entry, cancellationToken) =>
            await OrderOwnerAsync(entry.Context, entry.Entity.OrderId, cancellationToken) is Guid userId ? [userId] : [])
        .ForUserProperties<Invoice>(UserDataTopics.Orders, nameof(Invoice.UserId))
        .ForUserProperties<UserAddress>(UserDataTopics.Addresses, nameof(UserAddress.UserId));

    private static async Task<Guid?> BasketOwnerAsync(DbContext context, Guid basketId, CancellationToken cancellationToken) =>
        context.Set<Basket>().Local.FirstOrDefault(b => b.Id == basketId) is { } tracked
            ? tracked.UserId
            : await context.Set<Basket>().AsNoTracking().Where(b => b.Id == basketId).Select(b => b.UserId).FirstOrDefaultAsync(cancellationToken);

    private static async Task<Guid?> OrderOwnerAsync(DbContext context, Guid orderId, CancellationToken cancellationToken) =>
        context.Set<Order>().Local.FirstOrDefault(o => o.Id == orderId) is { } tracked
            ? tracked.UserId
            : await context.Set<Order>().AsNoTracking().Where(o => o.Id == orderId).Select(o => (Guid?)o.UserId).FirstOrDefaultAsync(cancellationToken);
}
