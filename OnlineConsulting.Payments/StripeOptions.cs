namespace OnlineConsulting.Payments;

/// <summary>Bound from the "Payment:Stripe" config section.</summary>
public class StripeOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;

    /// <summary>Signing secret for the separate "Connect" webhook endpoint in the Stripe dashboard, which carries connected-account events (account.updated etc.) - distinct from WebhookSecret, which only covers the platform's own account.</summary>
    public string ConnectWebhookSecret { get; set; } = string.Empty;

    /// <summary>OAuth client id for Connect Standard accounts (Dashboard > Settings > Connect > OAuth settings) - distinct from SecretKey, which authenticates the platform itself.</summary>
    public string ConnectClientId { get; set; } = string.Empty;
}
