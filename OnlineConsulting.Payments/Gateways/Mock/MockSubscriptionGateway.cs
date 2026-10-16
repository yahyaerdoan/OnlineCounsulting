using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Payments.Gateways.Mock;

/// <summary>No network calls - deterministic in-memory gateway for dev/testing without real provider credentials. Every call succeeds immediately.</summary>
public class MockSubscriptionGateway : ISubscriptionGateway
{
    private const string MockPricePrefix = "mock_price_";

    public string ProviderName => PaymentProviderNames.Mock;

    public bool SupportsMultipleItems => true;

    public Task<SubscriptionCustomerResult> EnsureCustomerAsync(EnsureCustomerRequest request, string? idempotencyKey = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionCustomerResult($"mock_cus_{request.ReferenceId}"));

    public Task<SubscriptionPriceResult> EnsurePriceAsync(EnsurePriceRequest request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionPriceResult($"mock_prod_{request.ReferenceId}", $"{MockPricePrefix}{request.ReferenceId}"));

    public Task<bool> PriceExistsAsync(string providerPriceId, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(providerPriceId.StartsWith(MockPricePrefix, StringComparison.Ordinal));

    public Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionRequest request, string? idempotencyKey = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionResult($"mock_sub_{request.ReferenceId}", PaymentStatuses.Succeeded, DateTimeOffset.UtcNow.AddMonths(1), FirstItemProviderId: NewItemId()));

    public Task<SubscriptionResult> CancelSubscriptionAsync(string providerSubscriptionId, bool atPeriodEnd = false, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(atPeriodEnd
            ? new SubscriptionResult(providerSubscriptionId, PaymentStatuses.Succeeded, DateTimeOffset.UtcNow.AddMonths(1))
            : new SubscriptionResult(providerSubscriptionId, PaymentStatuses.Refunded, DateTimeOffset.UtcNow));

    public Task<SubscriptionInvoice?> GetLatestInvoiceAsync(string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult<SubscriptionInvoice?>(null);

    public Task CancelAndRefundAsync(string providerCustomerId, string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<SubscriptionResult> UpdateSubscriptionPriceAsync(string providerSubscriptionId, string newProviderPriceId, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionResult(providerSubscriptionId, PaymentStatuses.Succeeded, DateTimeOffset.UtcNow.AddMonths(1)));

    public Task<SubscriptionResult> PauseSubscriptionAsync(string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionResult(providerSubscriptionId, PaymentStatuses.Succeeded, DateTimeOffset.UtcNow.AddMonths(1)));

    public Task<SubscriptionResult> ReactivateSubscriptionAsync(string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionResult(providerSubscriptionId, PaymentStatuses.Succeeded, DateTimeOffset.UtcNow.AddMonths(1)));

    public Task<SubscriptionResult> ResumeSubscriptionAsync(string providerSubscriptionId, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SubscriptionResult(providerSubscriptionId, PaymentStatuses.Succeeded, DateTimeOffset.UtcNow.AddMonths(1)));

    public Task<string> AddSubscriptionItemAsync(string providerSubscriptionId, string providerPriceId, string? idempotencyKey = null, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(NewItemId());

    public Task RemoveSubscriptionItemAsync(string providerSubscriptionItemId, string? connectedAccountId = null, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<SubscriptionWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default) =>
        Task.FromResult<SubscriptionWebhookEvent?>(null);

    /// <summary>Short like Stripe's ids, so it fits the 100-char columns.</summary>
    private static string NewItemId() => $"mock_si_{Guid.NewGuid():N}";
}
