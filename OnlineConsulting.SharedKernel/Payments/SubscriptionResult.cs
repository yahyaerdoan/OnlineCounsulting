namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>ClientSecret is null for Stripe (already attached server-side) or the PayPal approval URL the payer must be redirected to. FirstItemProviderId is the first line item's id, letting multi-item callers (Tenancy) capture it without a follow-up call.</summary>
public record SubscriptionResult(string ProviderSubscriptionId, string Status, DateTimeOffset CurrentPeriodEnd, string? ClientSecret = null, string? FirstItemProviderId = null);
