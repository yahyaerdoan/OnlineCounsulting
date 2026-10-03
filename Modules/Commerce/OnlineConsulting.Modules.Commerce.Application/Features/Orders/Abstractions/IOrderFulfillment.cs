using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;

/// <summary>
/// The one place a newly paid order is completed, whichever way the payment was confirmed (synchronous provider, webhook, payment-page
/// reconciliation or the background cleanup): clears the basket, issues the receipt invoice and notifies the customer.
/// </summary>
public interface IOrderFulfillment
{
    Task CompletePaidAsync(Order order, CancellationToken cancellationToken = default);
}
