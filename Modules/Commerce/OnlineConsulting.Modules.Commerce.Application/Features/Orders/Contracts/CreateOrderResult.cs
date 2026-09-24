namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

/// <summary>Carries the gateway's client secret alongside the order id so the client SDK can complete 3DS/SCA confirmation without a second round-trip.</summary>
public record CreateOrderResult(Guid OrderId, string? PaymentClientSecret, string OrderNumber);
