namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>ReferenceId round-trips whatever CreatePaymentIntentRequest.ReferenceId was, so the webhook handler can map back to the Order/Appointment without querying the provider for it.
/// ConnectedAccountId is set when the event came from a tenant's Stripe Connect account rather than the platform's, so the endpoint can resolve and scope to the owning tenant before publishing.</summary>
public record PaymentWebhookEvent(string ProviderPaymentId, string ReferenceId, bool Succeeded, string? ConnectedAccountId = null);
