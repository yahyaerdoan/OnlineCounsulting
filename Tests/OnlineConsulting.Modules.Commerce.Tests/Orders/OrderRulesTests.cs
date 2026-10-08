using OnlineConsulting.Modules.Commerce.Domain;

namespace OnlineConsulting.Modules.Commerce.Tests.Orders;

public class OrderRulesTests
{
    [Theory]
    [InlineData(OrderPaymentStatuses.Pending, OrderStatuses.Pending, true)]
    [InlineData(OrderPaymentStatuses.Pending, OrderStatuses.Cancelled, false)]
    [InlineData(OrderPaymentStatuses.Paid, OrderStatuses.Pending, false)]
    [InlineData(OrderPaymentStatuses.Cancelled, OrderStatuses.Cancelled, false)]
    [InlineData(OrderPaymentStatuses.Refunded, OrderStatuses.Pending, false)]
    public void IsAwaitingPayment(string paymentStatus, string orderStatus, bool expected) =>
        Assert.Equal(expected, OrderRules.IsAwaitingPayment(paymentStatus, orderStatus));

    [Theory]
    [InlineData(OrderPaymentStatuses.Paid, true)]
    [InlineData(OrderPaymentStatuses.Pending, false)]
    [InlineData(OrderPaymentStatuses.Cancelled, false)]
    [InlineData(OrderPaymentStatuses.Refunded, false)]
    public void CanBeRefunded(string paymentStatus, bool expected) =>
        Assert.Equal(expected, OrderRules.CanBeRefunded(paymentStatus));
}
