namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

/// <summary>Carries the gateway's client secret alongside the order id so the client SDK can complete 3DS/SCA confirmation without a second round-trip.
/// PaymentConnectedAccountId is the tenant's Stripe Connect account the PaymentIntent was created on - Stripe.js must be initialized with the same
/// account context (stripeAccount option) or the client secret won't resolve and the Payment Element silently fails to mount.</summary>
public record CreateOrderResult(Guid OrderId, string? PaymentClientSecret, string OrderNumber, string? PaymentConnectedAccountId = null);
