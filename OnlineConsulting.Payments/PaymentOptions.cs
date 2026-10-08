namespace OnlineConsulting.Payments;

/// <summary>Bound from the "Payment" config section. ActiveProvider is the one lever that switches providers - everything else stays wired regardless of which one is active, so flipping it back is just as cheap.</summary>
public class PaymentOptions
{
    public required string ActiveProvider { get; set; }
    public StripeOptions Stripe { get; set; } = new();
    public PayPalOptions PayPal { get; set; } = new();
}
