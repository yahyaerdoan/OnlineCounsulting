namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>One implementation per provider (Mock/Stripe/PayPal); swapping the active one is a config change (Payment:ActiveProvider), not a code change.</summary>
public interface IPaymentGateway
{
    /// <summary>Matches one of PaymentProviderNames - also the keyed-DI service key this implementation is registered under.</summary>
    string ProviderName { get; }

    /// <summary>Starts a payment for the given amount. Idempotent per idempotencyKey - retrying the same call (e.g. after a network timeout) must not create a second charge.</summary>
    Task<PaymentIntentResult> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, CancellationToken cancellationToken = default);

    Task<PaymentStatusResult> GetStatusAsync(string providerPaymentId, CancellationToken cancellationToken = default);

    /// <summary>amount null means a full refund.</summary>
    Task<PaymentStatusResult> RefundAsync(string providerPaymentId, decimal? amount = null, CancellationToken cancellationToken = default);

    /// <summary>Verifies the webhook signature and normalizes the payload; null if it's not a payment-status event (providers send many unrelated event types on the same endpoint).</summary>
    Task<PaymentWebhookEvent?> VerifyAndParseWebhookAsync(string rawBody, string? signatureHeader, CancellationToken cancellationToken = default);
}

public record CreatePaymentIntentRequest(decimal Amount, string Currency, string ReferenceId, string? CustomerEmail, string IdempotencyKey);

public record PaymentIntentResult(string ProviderPaymentId, string Status, string? ClientSecret);

/// <summary>ClientSecret is populated on a fresh retrieve so a caller can resume client-side payment
/// confirmation for an already-created intent (e.g. after a page reload) without creating a new one.</summary>
public record PaymentStatusResult(string ProviderPaymentId, string Status, string? ClientSecret = null);

/// <summary>ReferenceId round-trips whatever CreatePaymentIntentRequest.ReferenceId was, so the webhook handler can map back to the Order/Appointment without querying the provider for it.</summary>
public record PaymentWebhookEvent(string ProviderPaymentId, string ReferenceId, bool Succeeded);
