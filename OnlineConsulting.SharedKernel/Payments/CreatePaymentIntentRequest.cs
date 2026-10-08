namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>ReferenceId comes back on the webhook; IdempotencyKey makes a retried call safe.</summary>
public record CreatePaymentIntentRequest(decimal Amount, string Currency, string ReferenceId, string? CustomerEmail, string IdempotencyKey);
