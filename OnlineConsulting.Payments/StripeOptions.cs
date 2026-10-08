namespace OnlineConsulting.Payments;

/// <summary>Bound from the "Payment:Stripe" config section.</summary>
public class StripeOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}
