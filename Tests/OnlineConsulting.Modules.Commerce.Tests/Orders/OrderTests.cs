using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Tests.Orders;

public class OrderTests
{
    private static Order PlaceUnpaid() => Place(paidAtCheckout: false);

    private static Order Place(bool paidAtCheckout) => Order.Place(Guid.NewGuid(), "ORD-1", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Stripe",
        "pi_1", paidAtCheckout);

    [Fact]
    public void Place_WithoutSynchronousPayment_IsPendingAndAwaitingPayment()
    {
        var order = PlaceUnpaid();

        Assert.Equal(OrderStatuses.Pending, order.OrderStatus);
        Assert.Equal(OrderPaymentStatuses.Pending, order.PaymentStatus);
        Assert.True(order.IsAwaitingPayment);
        Assert.False(order.CanBeRefunded);
    }

    [Fact]
    public void Place_PaidAtCheckout_IsPaidAndRefundable()
    {
        var order = Place(paidAtCheckout: true);

        Assert.Equal(OrderPaymentStatuses.Paid, order.PaymentStatus);
        Assert.False(order.IsAwaitingPayment);
        Assert.True(order.CanBeRefunded);
    }

    [Fact]
    public void Place_KeepsGivenIdsAndPaymentReference()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var shippingId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();

        var order = Order.Place(id, "ORD-7", userId, shippingId, invoiceId, "Stripe", "pi_7", paidAtCheckout: false);

        Assert.Equal(id, order.Id);
        Assert.Equal("ORD-7", order.OrderNumber);
        Assert.Equal(userId, order.UserId);
        Assert.Equal(shippingId, order.ShippingAddressId);
        Assert.Equal(invoiceId, order.InvoiceAddressId);
        Assert.Equal("Stripe", order.PaymentProvider);
        Assert.Equal("pi_7", order.ProviderPaymentId);
    }

    [Theory]
    [InlineData("", "Stripe", "pi_1")]
    [InlineData("ORD-1", " ", "pi_1")]
    [InlineData("ORD-1", "Stripe", "")]
    public void Place_WithBlankRequiredValue_Throws(string orderNumber, string provider, string providerPaymentId)
        => Assert.ThrowsAny<ArgumentException>(() =>
            Order.Place(Guid.NewGuid(), orderNumber, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), provider, providerPaymentId, paidAtCheckout: false));

    [Fact]
    public void MarkPaid_WhenAwaitingPayment_SetsPaid()
    {
        var order = PlaceUnpaid();

        order.MarkPaid();

        Assert.Equal(OrderPaymentStatuses.Paid, order.PaymentStatus);
        Assert.Equal(OrderStatuses.Pending, order.OrderStatus);
        Assert.True(order.CanBeRefunded);
    }

    [Fact]
    public void MarkPaid_WhenAlreadyPaid_Throws()
        => Assert.Throws<InvalidOperationException>(Place(paidAtCheckout: true).MarkPaid);

    [Fact]
    public void MarkPaid_WhenCancelled_Throws()
    {
        var order = PlaceUnpaid();
        order.Cancel();

        _ = Assert.Throws<InvalidOperationException>(order.MarkPaid);
    }

    public static TheoryData<Action<Order>> CancellingActions => new()
    {
        o => o.Cancel(),
        o => o.FailPayment(),
        o => o.Abandon(),
    };

    [Theory]
    [MemberData(nameof(CancellingActions))]
    public void CancellingAction_WhenAwaitingPayment_CancelsOrderAndPayment(Action<Order> cancel)
    {
        var order = PlaceUnpaid();

        cancel(order);

        Assert.Equal(OrderStatuses.Cancelled, order.OrderStatus);
        Assert.Equal(OrderPaymentStatuses.Cancelled, order.PaymentStatus);
        Assert.False(order.IsAwaitingPayment);
        Assert.False(order.CanBeRefunded);
    }

    [Theory]
    [MemberData(nameof(CancellingActions))]
    public void CancellingAction_WhenPaid_ThrowsAndKeepsPayment(Action<Order> cancel)
    {
        var order = Place(paidAtCheckout: true);

        _ = Assert.Throws<InvalidOperationException>(() => cancel(order));
        Assert.Equal(OrderPaymentStatuses.Paid, order.PaymentStatus);
        Assert.Equal(OrderStatuses.Pending, order.OrderStatus);
    }

    [Theory]
    [MemberData(nameof(CancellingActions))]
    public void CancellingAction_WhenAlreadyCancelled_Throws(Action<Order> cancel)
    {
        var order = PlaceUnpaid();
        order.Cancel();

        _ = Assert.Throws<InvalidOperationException>(() => cancel(order));
    }

    [Fact]
    public void Refund_WhenPaid_SetsRefunded()
    {
        var order = Place(paidAtCheckout: true);

        order.Refund();

        Assert.Equal(OrderPaymentStatuses.Refunded, order.PaymentStatus);
        Assert.False(order.CanBeRefunded);
    }

    [Fact]
    public void Refund_WhenUnpaid_Throws()
        => Assert.Throws<InvalidOperationException>(PlaceUnpaid().Refund);

    [Fact]
    public void Refund_Twice_Throws()
    {
        var order = Place(paidAtCheckout: true);
        order.Refund();

        _ = Assert.Throws<InvalidOperationException>(order.Refund);
    }

    [Fact]
    public void ChangeAddresses_WhenAwaitingPayment_UpdatesBoth()
    {
        var order = PlaceUnpaid();
        var shippingId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();

        order.ChangeAddresses(shippingId, invoiceId);

        Assert.Equal(shippingId, order.ShippingAddressId);
        Assert.Equal(invoiceId, order.InvoiceAddressId);
    }

    [Fact]
    public void ChangeAddresses_WhenPaid_ThrowsAndKeepsAddresses()
    {
        var order = Place(paidAtCheckout: true);
        var shippingId = order.ShippingAddressId;

        _ = Assert.Throws<InvalidOperationException>(() => order.ChangeAddresses(Guid.NewGuid(), Guid.NewGuid()));
        Assert.Equal(shippingId, order.ShippingAddressId);
    }
}
