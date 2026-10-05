namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Finds or creates the provider product and recurring price for ReferenceId; BillingCycle is one of BillingCycles.</summary>
public record EnsurePriceRequest(string ReferenceId, string Name, decimal Amount, string Currency, string BillingCycle);
