namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Links a tenant's own, independently-owned payment account (Stripe Connect Standard - the tenant brings/creates their own account via OAuth) so customer-facing
/// charges (orders, memberships) route to the tenant rather than the platform. One implementation per provider, Mock/Stripe today - PayPal has no Connect-equivalent wired up yet.
/// Standard (not Express/Custom) because Stripe no longer allows direct charges to platform-created Express/Custom accounts for new platforms.</summary>
public interface IConnectAccountGateway
{
    /// <summary>Matches one of PaymentProviderNames - also the keyed-DI service key this implementation is registered under.</summary>
    string ProviderName { get; }

    /// <summary>The provider-hosted OAuth consent page to redirect the tenant admin to; state round-trips unchanged to the callback (CSRF protection + carries which tenant/where to return to).</summary>
    string BuildAuthorizeUrl(string state, string redirectUri);

    /// <summary>Exchanges the OAuth callback's authorization code for the tenant's own connected account id.</summary>
    Task<ConnectAccountResult> ExchangeOAuthCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Live capability check - used by the onboarding return page to reflect status immediately, without waiting for the webhook.</summary>
    Task<ConnectAccountStatus> GetAccountStatusAsync(string accountId, CancellationToken cancellationToken = default);

    /// <summary>Verifies the webhook signature and normalizes the payload; null if it's not an account-capability event.</summary>
    Task<ConnectAccountWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default);
}
