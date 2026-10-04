using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>
/// A checkout and its payment, the aggregate root of its <see cref="Items"/>. State changes only through its methods, which throw when called in the
/// wrong state; load it with its items when they're needed and save them together.
/// </summary>
public class Order : SequentialGuidTenantEntity
{
    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    public string OrderNumber { get; private set; } = string.Empty;
    public string OrderStatus { get; private set; } = OrderStatuses.Pending;
    public string PaymentStatus { get; private set; } = OrderPaymentStatuses.Pending;

    /// <summary>The gateway that took the payment (PaymentProviderNames.*); refunds go back to it.</summary>
    public string? PaymentProvider { get; private set; }
    public string? ProviderPaymentId { get; private set; }

    /// <summary>Identity module user id, no navigation.</summary>
    public Guid UserId { get; private set; }

    public Guid ShippingAddressId { get; private set; }
    public Guid InvoiceAddressId { get; private set; }

    /// <summary>What was bought, priced when the order was placed; fixed afterwards.</summary>
    public IReadOnlyList<OrderItem> Items => _items;

    /// <summary>Unpaid and not cancelled: can be paid, cancelled or have its addresses changed.</summary>
    public bool IsAwaitingPayment => OrderRules.IsAwaitingPayment(PaymentStatus, OrderStatus);

    /// <summary>Paid, so it can be refunded.</summary>
    public bool CanBeRefunded => OrderRules.CanBeRefunded(PaymentStatus);

    /// <summary>Creates a pending order from the basket's (already repriced) items; <paramref name="paidAtCheckout"/> when the gateway settled the payment immediately.</summary>
    public static Order Place(Guid id, string orderNumber, Guid userId, Guid shippingAddressId, Guid invoiceAddressId, string paymentProvider, string providerPaymentId,
        bool paidAtCheckout, IReadOnlyCollection<BasketItem> basketItems)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(orderNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentProvider);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerPaymentId);

        if (basketItems.Count == 0)
        {
            throw new ArgumentException("An order needs at least one item.", nameof(basketItems));
        }

        var order = new Order
        {
            Id = id,
            OrderNumber = orderNumber,
            UserId = userId,
            ShippingAddressId = shippingAddressId,
            InvoiceAddressId = invoiceAddressId,
            PaymentProvider = paymentProvider,
            ProviderPaymentId = providerPaymentId,
            PaymentStatus = paidAtCheckout ? OrderPaymentStatuses.Paid : OrderPaymentStatuses.Pending,
        };

        order._items.AddRange(basketItems.Select(item => OrderItem.Create(id, item.ServiceId, item.Quantity, item.Price, item.TaxRate)));

        return order;
    }

    /// <summary>Records a successful payment. Requires <see cref="IsAwaitingPayment"/>.</summary>
    public void MarkPaid()
    {
        EnsureAwaitingPayment(nameof(MarkPaid));
        PaymentStatus = OrderPaymentStatuses.Paid;
    }

    /// <summary>Cancels the order because the gateway refused the payment. Requires <see cref="IsAwaitingPayment"/>.</summary>
    public void FailPayment() => CancelUnpaid(nameof(FailPayment));

    /// <summary>Cancels the order at the customer's request. Requires <see cref="IsAwaitingPayment"/>.</summary>
    public void Cancel() => CancelUnpaid(nameof(Cancel));

    /// <summary>Cancels the order because no payment arrived in time. Requires <see cref="IsAwaitingPayment"/>.</summary>
    public void Abandon() => CancelUnpaid(nameof(Abandon));

    /// <summary>Records a refund. Requires <see cref="CanBeRefunded"/>.</summary>
    public void Refund()
    {
        if (!CanBeRefunded)
        {
            throw new InvalidOperationException($"Order {OrderNumber} cannot be refunded from payment status {PaymentStatus}.");
        }

        PaymentStatus = OrderPaymentStatuses.Refunded;
    }

    /// <summary>Replaces both addresses. Requires <see cref="IsAwaitingPayment"/>.</summary>
    public void ChangeAddresses(Guid shippingAddressId, Guid invoiceAddressId)
    {
        EnsureAwaitingPayment(nameof(ChangeAddresses));
        ShippingAddressId = shippingAddressId;
        InvoiceAddressId = invoiceAddressId;
    }

    private void CancelUnpaid(string action)
    {
        EnsureAwaitingPayment(action);
        PaymentStatus = OrderPaymentStatuses.Cancelled;
        OrderStatus = OrderStatuses.Cancelled;
    }

    private void EnsureAwaitingPayment(string action)
    {
        if (!IsAwaitingPayment)
        {
            throw new InvalidOperationException($"{action} needs an unpaid order, but order {OrderNumber} is {OrderStatus} / {PaymentStatus}.");
        }
    }
}
