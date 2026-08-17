using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Cart;

public class CartService(IApiClient apiClient) : ICartService
{
    private const string BasketPath = "/api/basket";

    public async Task<CartResponse?> GetAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<CartResponse>(BasketPath, cancellationToken);
        return result.ResultData;
    }

    public async Task<int> GetItemsCountAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<int>($"{BasketPath}/count", cancellationToken);
        return result.ResultData;
    }

    // UserId/GuestId are resolved and overwritten server-side by BasketOwnerResolver - not sent from here.
    public Task<ApiEnvelope> AddItemAsync(Guid serviceId, int quantity, decimal price, int taxRate, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{BasketPath}/items", new { ServiceId = serviceId, Quantity = quantity, Price = price, TaxRate = taxRate }, cancellationToken);

    public Task<ApiEnvelope> RemoveItemAsync(Guid itemId, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{BasketPath}/items/{itemId}", cancellationToken);

    public Task<ApiEnvelope> ClearAsync(CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync(BasketPath, cancellationToken);
}
