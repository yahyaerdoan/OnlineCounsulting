using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Cart;

/// <summary>Wraps /api/basket* - no cookie/guest-id handling here, GuestIdHandler bridges the Api's guest_id
/// cookie transparently for every IApiClient call, authenticated or anonymous.</summary>
public interface ICartService
{
    /// <summary>Gets the current cart, or null if none exists.</summary>
    Task<CartResponse?> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the current cart's item count.</summary>
    Task<int> GetItemsCountAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a line item to the cart.</summary>
    Task<ApiEnvelope> AddItemAsync(Guid serviceId, int quantity, decimal price, int taxRate, CancellationToken cancellationToken = default);

    /// <summary>Removes a line item from the cart.</summary>
    Task<ApiEnvelope> RemoveItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>Clears the entire cart.</summary>
    Task<ApiEnvelope> ClearAsync(CancellationToken cancellationToken = default);
}

public record CartItemResponse(Guid Id, Guid ServiceId, int Quantity, decimal Price, int TaxRate, decimal TaxAmount, decimal SubTotalPrice, decimal TotalPrice);

public record CartResponse(Guid Id, int Quantity, decimal SubTotalPrice, decimal TotalPrice, List<CartItemResponse> Items);
