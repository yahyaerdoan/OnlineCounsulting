namespace OnlineConsulting.Modules.Commerce.Domain;

/// <summary>Values of <see cref="Order.PaymentStatus"/>; not the gateway's SharedKernel PaymentStatuses.</summary>
public static class OrderPaymentStatuses
{
    public const string Pending = "Pending";
    public const string Paid = "Paid";
    public const string Cancelled = "Cancelled";
    public const string Refunded = "Refunded";
}
