namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>ClientSecret is null for Stripe (already attached server-side) or the PayPal approval URL the payer must be redirected to. ItemIdsByPriceId maps each created line item's price to its item id, so multi-item callers (Tenancy) need no follow-up call.</summary>
public record SubscriptionResult(string ProviderSubscriptionId, string Status, DateTimeOffset CurrentPeriodEnd, string? ClientSecret = null, IReadOnlyDictionary<string, string>? ItemIdsByPriceId = null);
