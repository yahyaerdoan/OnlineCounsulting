namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>ReferenceId round-trips whatever CreatePaymentIntentRequest.ReferenceId was, so the webhook handler can map back to the Order/Appointment without querying the provider for it.</summary>
public record PaymentWebhookEvent(string ProviderPaymentId, string ReferenceId, bool Succeeded);
