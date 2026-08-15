namespace OnlineConsulting.Payments;

/// <summary>Bound from the "Payment" config section. ActiveProvider is the one lever that switches providers - everything else stays wired regardless of which one is active, so flipping it back is just as cheap.</summary>
public class PaymentOptions
{
    public required string ActiveProvider { get; set; }
    public StripeOptions Stripe { get; set; } = new();
    public PayPalOptions PayPal { get; set; } = new();
}

public class StripeOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}

public class PayPalOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public bool UseSandbox { get; set; } = true;
}
