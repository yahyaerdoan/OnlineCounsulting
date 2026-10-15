namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>One implementation per provider (Mock/Stripe/PayPal); swapping the active one is a config change (Payment:ActiveProvider), not a code change.</summary>
public interface IPaymentGateway
{
    /// <summary>Matches one of PaymentProviderNames - also the keyed-DI service key this implementation is registered under.</summary>
    string ProviderName { get; }

    /// <summary>Starts a payment for the given amount. Idempotent per idempotencyKey - retrying the same call (e.g. after a network timeout) must not create a second charge.</summary>
    Task<PaymentIntentResult> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken = default);

    /// <summary>The payment's current status at the provider. connectedAccountId must match whatever CreatePaymentIntentAsync used for this payment, or the provider can't find it.</summary>
    Task<PaymentStatusResult> GetStatusAsync(string providerPaymentId, string? connectedAccountId = null, CancellationToken cancellationToken = default);

    /// <summary>amount null means a full refund; connectedAccountId - see GetStatusAsync.</summary>
    Task<PaymentStatusResult> RefundAsync(string providerPaymentId, decimal? amount = null, string? connectedAccountId = null, CancellationToken cancellationToken = default);

    /// <summary>Verifies the webhook signature and normalizes the payload; null if it's not a payment-status event (providers send many unrelated event types on the same endpoint).</summary>
    Task<PaymentWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default);
}
