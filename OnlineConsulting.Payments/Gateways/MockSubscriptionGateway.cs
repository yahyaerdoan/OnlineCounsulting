using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Payments.Gateways;

/// <summary>No network calls - deterministic in-memory gateway for dev/testing without real provider credentials. Every call succeeds immediately.</summary>
public class MockSubscriptionGateway : ISubscriptionGateway
{
    public string ProviderName => PaymentProviderNames.Mock;

    public Task<SubscriptionCustomerResult> EnsureCustomerAsync(EnsureCustomerRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionCustomerResult($"mock_cus_{request.ReferenceId}"));

    public Task<SubscriptionPriceResult> EnsurePriceAsync(EnsurePriceRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionPriceResult($"mock_prod_{request.ReferenceId}", $"mock_price_{request.ReferenceId}"));

    public Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionResult($"mock_sub_{request.ReferenceId}", PaymentStatuses.Succeeded, DateTimeOffset.UtcNow.AddMonths(1), FirstItemProviderId: $"mock_si_{request.ReferenceId}_{request.ProviderPriceId}"));

    public Task<SubscriptionResult> CancelSubscriptionAsync(string providerSubscriptionId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionResult(providerSubscriptionId, PaymentStatuses.Refunded, DateTimeOffset.UtcNow));

    public Task<string> AddSubscriptionItemAsync(string providerSubscriptionId, string providerPriceId, CancellationToken cancellationToken = default) =>
        Task.FromResult($"mock_si_{providerSubscriptionId}_{providerPriceId}");

    public Task RemoveSubscriptionItemAsync(string providerSubscriptionItemId, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<SubscriptionWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default) =>
        Task.FromResult<SubscriptionWebhookEvent?>(null);
}
