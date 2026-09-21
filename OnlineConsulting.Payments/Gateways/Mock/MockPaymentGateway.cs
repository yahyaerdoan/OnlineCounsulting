using OnlineConsulting.SharedKernel.Payments;
using System.Collections.Concurrent;

namespace OnlineConsulting.Payments.Gateways.Mock;

/// <summary>Deterministic in-memory gateway for dev/testing - amounts ending in .00 succeed immediately, anything else stays Pending so callers can exercise both paths without a sandbox account.</summary>
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
