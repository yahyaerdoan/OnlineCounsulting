using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Payments.Gateways.Mock;

/// <summary>No network calls - the "authorize" step is skipped and every code exchanges to a deterministic fake account id, so dev/testing can exercise the tenant-routed checkout path without a real Connect account.</summary>
public class MockConnectAccountGateway : IConnectAccountGateway
{
    public string ProviderName => PaymentProviderNames.Mock;

    public string BuildAuthorizeUrl(string state, string redirectUri) =>
        $"{redirectUri}?code=mock_code&state={Uri.EscapeDataString(state)}";

    public Task<ConnectAccountResult> ExchangeOAuthCodeAsync(string code, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ConnectAccountResult($"mock_acct_{code}"));

    public Task<ConnectAccountStatus> GetAccountStatusAsync(string accountId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ConnectAccountStatus(ChargesEnabled: true, PayoutsEnabled: true));

    public Task<ConnectAccountWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default) =>
        Task.FromResult<ConnectAccountWebhookEvent?>(null);
}
