namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Provider-agnostic status a gateway result is normalized into - every provider's own status vocabulary (Stripe's "requires_confirmation"/"succeeded", PayPal's "CREATED"/"COMPLETED") maps into one of these so callers never branch on a provider-specific string.</summary>
public static class PaymentStatuses
{
    public const string Pending = "Pending";
    public const string Succeeded = "Succeeded";
    public const string Failed = "Failed";
    public const string Refunded = "Refunded";
}
