using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Cart;

/// <summary>Composes ICartService with IServiceCatalogService so CartController calls only this one interface; works anonymously via GuestIdHandler's guest_id cookie bridge.</summary>
public interface ICartPageService
{
    /// <summary>Gets the current cart, or null if empty/none exists.</summary>
    Task<CartViewModel?> GetCartAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a service to the cart by slug.</summary>
    Task<ApiEnvelope> AddToCartAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Removes a line item from the cart.</summary>
    Task<ApiEnvelope> RemoveItemAsync(Guid itemId, CancellationToken cancellationToken = default);
}
