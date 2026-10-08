namespace OnlineConsulting.Payments;

/// <summary>Bound from the "Payment:PayPal" config section.</summary>
public class PayPalOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public bool UseSandbox { get; set; } = true;

    /// <summary>Where PayPal redirects the payer after they approve/cancel a subscription - required by the Subscriptions API to generate an approval link at all.</summary>
    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}
