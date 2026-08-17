using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Cart;

/// <summary>Wraps /api/basket* - no cookie/guest-id handling here, GuestIdHandler bridges the Api's guest_id
/// cookie transparently for every IApiClient call, authenticated or anonymous.</summary>
public interface ICartService
{
    Task<CartResponse?> GetAsync(CancellationToken cancellationToken = default);
    Task<int> GetItemsCountAsync(CancellationToken cancellationToken = default);
    Task<ApiEnvelope> AddItemAsync(Guid serviceId, int quantity, decimal price, int taxRate, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> RemoveItemAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<ApiEnvelope> ClearAsync(CancellationToken cancellationToken = default);
}

public record CartItemResponse(Guid Id, Guid ServiceId, int Quantity, decimal Price, int TaxRate, decimal TaxAmount, decimal SubTotalPrice, decimal TotalPrice);

public record CartResponse(Guid Id, int Quantity, decimal SubTotalPrice, decimal TotalPrice, List<CartItemResponse> Items);
