using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Checkout;

public class CheckoutService(IApiClient apiClient) : ICheckoutService
{
    // UserId/Email are resolved server-side from the current user - nothing to send from here.
    public Task<ApiEnvelope<CheckoutResult>> CreateOrderFromBasketAsync(CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<CheckoutResult>("/api/orders/checkout", null, cancellationToken);
}
