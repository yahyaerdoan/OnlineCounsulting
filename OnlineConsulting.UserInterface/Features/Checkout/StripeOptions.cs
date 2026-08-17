namespace OnlineConsulting.UserInterface.Features.Checkout;

/// <summary>UI-local replacement for the legacy OnlineConsulting.BusinessLogic AppSettingStripeOption - only
/// PublishableKey is used here (the secret key/actual payment intent creation lives server-side in the Payments
/// module, see Modules/Commerce's IPaymentGateway). Bound from the same "Stripe" config section.</summary>
public class StripeOptions
{
    public const string SectionName = "Stripe";
    public string PublishableKey { get; set; } = string.Empty;
}
