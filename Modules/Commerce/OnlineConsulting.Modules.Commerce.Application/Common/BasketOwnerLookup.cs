using OnlineConsulting.Modules.Commerce.Application.Features.Baskets.Abstractions;
using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Common;

public static class BasketOwnerLookup
{
    /// <summary>The owner's basket with its items, opening an empty one if they don't have one yet.</summary>
    public static async Task<Basket> GetOrOpenAsync(IBasketRepository basketRepository, Guid? userId, Guid? guestId, CancellationToken cancellationToken) =>
        await basketRepository.GetForOwnerAsync(userId, guestId, cancellationToken: cancellationToken)
        ?? await basketRepository.AddAsync(Basket.Open(userId, guestId), cancellationToken: cancellationToken);
}
