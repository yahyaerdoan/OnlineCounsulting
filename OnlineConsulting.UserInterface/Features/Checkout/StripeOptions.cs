namespace OnlineConsulting.UserInterface.Features.Checkout;

/// <summary>Only PublishableKey lives here - the secret key and payment intent creation stay server-side in the Payments module.</summary>
public class StripeOptions
{
    public const string SectionName = "Stripe";
    public string PublishableKey { get; set; } = string.Empty;
}
