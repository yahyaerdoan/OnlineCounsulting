namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>DiscountAmount is a one-time first-invoice discount, not a recurring price change; TrialDays delays the first real charge. Both ignored by providers with no such concept (PayPal).</summary>
/// <summary>ConnectedAccountId - see CreatePaymentIntentRequest. AdditionalPriceIds are billed on the same first invoice (multi-item providers only).</summary>
public record CreateSubscriptionRequest(string ProviderCustomerId, string ProviderPriceId, string PaymentMethodId, string ReferenceId, decimal? DiscountAmount = null, int? TrialDays = null, string? ConnectedAccountId = null,
    IReadOnlyList<string>? AdditionalPriceIds = null);
