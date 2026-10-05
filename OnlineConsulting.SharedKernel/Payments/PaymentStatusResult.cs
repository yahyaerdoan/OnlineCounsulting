namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>ClientSecret is populated on a fresh retrieve so a caller can resume client-side payment
/// confirmation for an already-created intent (e.g. after a page reload) without creating a new one.</summary>
public record PaymentStatusResult(string ProviderPaymentId, string Status, string? ClientSecret = null);
