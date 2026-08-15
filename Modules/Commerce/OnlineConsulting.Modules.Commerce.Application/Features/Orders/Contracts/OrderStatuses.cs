namespace OnlineConsulting.Modules.Commerce.Application.Features.Orders.Contracts;

public static class OrderStatuses
{
    public const string Pending = "Pending";
    public const string Cancelled = "Cancelled";
}

public static class PaymentStatuses
{
    public const string Pending = "Pending";
    public const string Paid = "Paid";
    public const string Cancelled = "Cancelled";
    public const string Refunded = "Refunded";
}
