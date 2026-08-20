using OnlineConsulting.UserInterface.Features.Service;
using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Cart;

public class CartPageService(ICartService cartService, IServiceCatalogService serviceCatalogService) : ICartPageService
{
    public async Task<CartViewModel?> GetCartAsync(CancellationToken cancellationToken = default)
    {
        var cart = await cartService.GetAsync(cancellationToken);
        if (cart is null)
        {
            return null;
        }

        var lines = new List<CartLineViewModel>();
        foreach (var item in cart.Items)
        {
            var service = await serviceCatalogService.GetByIdAsync(item.ServiceId, cancellationToken);
            lines.Add(new CartLineViewModel(item.Id, service?.Title ?? string.Empty, item.Quantity, item.Price, item.TaxRate, item.TaxAmount, item.SubTotalPrice, item.TotalPrice));
        }

        return new CartViewModel(cart.Id, lines, cart.SubTotalPrice, cart.TotalPrice);
    }

    public async Task<ApiEnvelope> AddToCartAsync(string slug, CancellationToken cancellationToken = default)
    {
        var service = await serviceCatalogService.GetBySlugAsync(slug, cancellationToken);
        if (service is null)
        {
            return new ApiEnvelope(false, StatusCodes.Status404NotFound, "Service not found.", null);
        }

        // Priced at the service's already-discounted price, matching what the customer actually pays.
        return await cartService.AddItemAsync(service.Id, 1, service.DiscountedPrice, service.TaxRate, cancellationToken);
    }

    public Task<ApiEnvelope> RemoveItemAsync(Guid itemId, CancellationToken cancellationToken = default) =>
        cartService.RemoveItemAsync(itemId, cancellationToken);
}
