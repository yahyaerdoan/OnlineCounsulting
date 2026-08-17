namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

/// <summary>Checkout response - carries the payment gateway's client secret alongside the new order id so Stripe.js (or another provider's client SDK) can complete client-side payment confirmation (3DS/SCA) without a second round-trip to look it up.</summary>
public record CreateOrderResult(Guid OrderId, string? PaymentClientSecret, string OrderNumber);
