using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Payments.Gateways.PayPal;

/// <summary>PayPal's marketplace equivalent (Partner Referrals / multiparty) isn't wired up - tenant-routed payouts aren't available while PayPal is the active provider.</summary>
public class PayPalConnectAccountGateway : IConnectAccountGateway
{
    public string ProviderName => PaymentProviderNames.PayPal;

    public string BuildAuthorizeUrl(string state, string redirectUri) =>
        throw new NotSupportedException("PayPal Partner Referrals (marketplace payouts) is not implemented.");

    public Task<ConnectAccountResult> ExchangeOAuthCodeAsync(string code, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("PayPal Partner Referrals (marketplace payouts) is not implemented.");

    public Task<ConnectAccountStatus> GetAccountStatusAsync(string accountId, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("PayPal Partner Referrals (marketplace payouts) is not implemented.");

    public Task<ConnectAccountWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException("PayPal Partner Referrals (marketplace payouts) is not implemented.");
}
