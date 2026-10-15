namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Finds or creates the provider customer for ReferenceId.</summary>
/// <summary>ConnectedAccountId - see CreatePaymentIntentRequest.</summary>
public record EnsureCustomerRequest(string ReferenceId, string Email, string? ConnectedAccountId = null);
