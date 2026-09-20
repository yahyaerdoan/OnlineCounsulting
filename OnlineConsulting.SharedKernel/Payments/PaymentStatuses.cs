namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Provider-agnostic status every gateway result normalizes to, so callers never branch on a provider-specific string (Stripe's "succeeded", PayPal's "COMPLETED", etc).</summary>
public static class PaymentStatuses
{
    public const string Pending = "Pending";
    public const string Succeeded = "Succeeded";
    public const string Failed = "Failed";
    public const string Refunded = "Refunded";
}
