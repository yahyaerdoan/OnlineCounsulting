using Microsoft.Extensions.Options;
using OnlineConsulting.SharedKernel.Payments;
using Stripe;

namespace OnlineConsulting.Payments.Gateways.Stripe;

/// <summary>Connect Standard: the tenant brings/creates their own full Stripe account and authorizes the platform via OAuth - never created or onboarded by this gateway,
/// unlike Express/Custom (which Stripe no longer allows direct charges against for new platforms).</summary>
public class StripeConnectAccountGateway(IOptions<PaymentOptions> options) : IConnectAccountGateway
{
    private readonly StripeClient _client = new(options.Value.Stripe.SecretKey);
    private readonly string _clientId = options.Value.Stripe.ConnectClientId;
    private readonly string _webhookSecret = options.Value.Stripe.ConnectWebhookSecret;

    public string ProviderName => PaymentProviderNames.Stripe;

    public string BuildAuthorizeUrl(string state, string redirectUri)
    {
        var service = new OAuthTokenService(_client);

        return service.AuthorizeUrl(new OAuthAuthorizeUrlOptions
        {
            ClientId = _clientId,
            RedirectUri = redirectUri,
            Scope = "read_write",
            State = state,
        }).ToString();
    }

    public async Task<ConnectAccountResult> ExchangeOAuthCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var service = new OAuthTokenService(_client);

        var token = await service.CreateAsync(new OAuthTokenCreateOptions
        {
            GrantType = "authorization_code",
            Code = code,
        }, cancellationToken: cancellationToken);

        return new ConnectAccountResult(token.StripeUserId);
    }

    public async Task<ConnectAccountStatus> GetAccountStatusAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var service = new AccountService(_client);
        var account = await service.GetAsync(accountId, cancellationToken: cancellationToken);

        return new ConnectAccountStatus(account.ChargesEnabled, account.PayoutsEnabled);
    }

    /// <summary>Verified against ConnectWebhookSecret, not WebhookSecret - the Connect webhook endpoint is a separate endpoint in the Stripe dashboard with its own signing secret.</summary>
    public Task<ConnectAccountWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(signatureHeader) || string.IsNullOrEmpty(rawBody))
        {
            return Task.FromResult<ConnectAccountWebhookEvent?>(null);
        }

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(rawBody, signatureHeader, _webhookSecret);
        }
        catch (Exception)
        {
            return Task.FromResult<ConnectAccountWebhookEvent?>(null);
        }

        if (stripeEvent.Type != "account.updated" || stripeEvent.Data.Object is not Account account)
        {
            return Task.FromResult<ConnectAccountWebhookEvent?>(null);
        }

        return Task.FromResult<ConnectAccountWebhookEvent?>(new ConnectAccountWebhookEvent(account.Id, account.ChargesEnabled, account.PayoutsEnabled));
    }
}
