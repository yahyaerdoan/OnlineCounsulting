using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;
using System.Linq.Expressions;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

/// <summary>Looks up a basket by whichever single owner (UserId xor GuestId) the caller has, shared by every Basket handler.</summary>
public static class BasketOwnerLookup
{
    public static Expression<Func<Basket, bool>> Predicate(Guid? userId, Guid? guestId) => userId is { } uid ? b => b.UserId == uid : b => b.GuestId == guestId;

    /// <summary>Finds the owner's basket, creating an empty one if they don't have one yet.</summary>
    public static async Task<Basket> GetOrCreateAsync(IBasketRepository basketRepository, Guid? userId, Guid? guestId, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetAsync(Predicate(userId, guestId), cancellationToken: cancellationToken);
        if (basket is not null)
        {
            return basket;
        }

        basket = new Basket { UserId = userId, GuestId = guestId };
        _ = await basketRepository.AddAsync(basket);
        return basket;
    }
}
