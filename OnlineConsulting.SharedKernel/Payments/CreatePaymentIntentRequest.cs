namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>ReferenceId comes back on the webhook; IdempotencyKey makes a retried call safe.</summary>
/// <summary>ConnectedAccountId routes the charge to a tenant's own Stripe Connect account instead of the platform's; null keeps it on the platform (e.g. the tenant's own SaaS billing).</summary>
public record CreatePaymentIntentRequest(decimal Amount, string Currency, string ReferenceId, string? CustomerEmail, string IdempotencyKey, string? ConnectedAccountId = null);
