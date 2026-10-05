namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>The provider ids of the product and its recurring price.</summary>
public record SubscriptionPriceResult(string ProviderProductId, string ProviderPriceId);
