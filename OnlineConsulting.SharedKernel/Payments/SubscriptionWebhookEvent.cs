namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>ReferenceId round-trips CreateSubscriptionRequest.ReferenceId so the webhook handler can map back without querying the provider; NewRenewalDate is set only when EventKind is Renewed.
/// ConnectedAccountId - see PaymentWebhookEvent.</summary>
public record SubscriptionWebhookEvent(string ProviderSubscriptionId, string ReferenceId, string EventKind, DateTimeOffset? NewRenewalDate = null, SubscriptionInvoice? Invoice = null, string? ConnectedAccountId = null);
