using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Abstractions;

/// <summary>
/// Tells the customer about every order change they did not make themselves: an email and an in-app notification (which also goes out as a
/// push when a device is registered). Best effort: a failed send is logged and never fails the operation that triggered it.
/// </summary>
public interface IOrderNotifier
{
    Task PaidAsync(Order order, int itemCount, decimal total, Guid invoiceId, CancellationToken cancellationToken = default);

    Task PaymentFailedAsync(Order order, CancellationToken cancellationToken = default);

    Task AbandonedAsync(Order order, CancellationToken cancellationToken = default);

    Task RefundedAsync(Order order, decimal? amount, CancellationToken cancellationToken = default);
}
