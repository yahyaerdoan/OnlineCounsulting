using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Features.Checkout;

/// <summary>Places an order from the current user's basket via /api/orders/checkout. Address selection is the
/// caller's job via IUserAddressService - the Api uses whatever the user currently has marked as billing/shipping.</summary>
public interface ICheckoutService
{
    Task<ApiEnvelope<CheckoutResult>> CreateOrderFromBasketAsync(CancellationToken cancellationToken = default);
}

/// <summary>Mirrors the Api's CreateOrderResult - PaymentClientSecret is null once a gateway already settled the
/// payment synchronously (e.g. Mock), populated when the client still needs to confirm (e.g. Stripe 3DS/SCA).</summary>
public record CheckoutResult(Guid OrderId, string? PaymentClientSecret, string OrderNumber);
