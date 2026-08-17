using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Cart;

/// <summary>Api orchestration for the public cart page - composes ICartService (basket) with
/// IServiceCatalogService (slug->id lookup on add, service title resolution for line items) so CartController only
/// ever calls this one interface. Works anonymously too - ICartService/IApiClient already bridge the Api's
/// guest_id cookie transparently via GuestIdHandler, nothing guest-specific needed here.</summary>
public interface ICartPageService
{
    Task<CartViewModel?> GetCartAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> AddToCartAsync(string slug, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> RemoveItemAsync(Guid itemId, CancellationToken cancellationToken = default);
}
