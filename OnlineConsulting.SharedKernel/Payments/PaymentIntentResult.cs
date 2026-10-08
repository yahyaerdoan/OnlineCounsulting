namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>ClientSecret lets the client confirm the payment; Status is one of PaymentStatuses.</summary>
public record PaymentIntentResult(string ProviderPaymentId, string Status, string? ClientSecret);
