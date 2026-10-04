namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>Order lifecycle rules shared by <see cref="Order"/> and code that only has the statuses (e.g. HATEOAS links).</summary>
public static class OrderRules
{
    /// <summary>Unpaid and not cancelled.</summary>
    public static bool IsAwaitingPayment(string paymentStatus, string orderStatus) =>
        paymentStatus == OrderPaymentStatuses.Pending && orderStatus != OrderStatuses.Cancelled;

    /// <summary>Only a paid order can be refunded.</summary>
    public static bool CanBeRefunded(string paymentStatus) => paymentStatus == OrderPaymentStatuses.Paid;
}
