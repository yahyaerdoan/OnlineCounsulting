using OnlineConsulting.SharedKernel.Payments;
using System.Collections.Concurrent;

namespace OnlineConsulting.Payments.Gateways.Mock;

/// <summary>No network calls - deterministic in-memory gateway for dev/testing without real provider credentials. Amounts ending in .00 succeed immediately; anything else is left Pending, mirroring how a real card can be declined or need extra confirmation, so callers can exercise both paths without a sandbox account.</summary>
public class MockPaymentGateway : IPaymentGateway
{
    private static readonly ConcurrentDictionary<string, PaymentStatusResult> _payments = new();

    public string ProviderName => PaymentProviderNames.Mock;

    public Task<PaymentIntentResult> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken = default)
    {
        var providerPaymentId = $"mock_{request.IdempotencyKey}";
        var status = request.Amount % 1 == 0 ? PaymentStatuses.Succeeded : PaymentStatuses.Pending;

        _payments[providerPaymentId] = new PaymentStatusResult(providerPaymentId, status, $"mock_secret_{providerPaymentId}");

        return Task.FromResult(new PaymentIntentResult(providerPaymentId, status, ClientSecret: $"mock_secret_{providerPaymentId}"));
    }

    public Task<PaymentStatusResult> GetStatusAsync(string providerPaymentId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_payments.GetValueOrDefault(providerPaymentId, new PaymentStatusResult(providerPaymentId, PaymentStatuses.Failed)));

    public Task<PaymentStatusResult> RefundAsync(string providerPaymentId, decimal? amount = null, CancellationToken cancellationToken = default)
    {
        var result = new PaymentStatusResult(providerPaymentId, PaymentStatuses.Refunded);
        _payments[providerPaymentId] = result;
        return Task.FromResult(result);
    }

    public Task<PaymentWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default) =>
        Task.FromResult<PaymentWebhookEvent?>(null);
}
