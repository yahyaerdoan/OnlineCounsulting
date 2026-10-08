namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>SubscriptionWebhookEvent.EventKind values.</summary>
public static class SubscriptionEventKinds
{
    public const string Renewed = "Renewed";
    public const string Cancelled = "Cancelled";
    public const string PaymentFailed = "PaymentFailed";
}
