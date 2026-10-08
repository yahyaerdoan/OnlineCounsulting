namespace OnlineConsulting.SharedKernel.Payments;

/// <summary>Finds or creates the provider customer for ReferenceId.</summary>
public record EnsureCustomerRequest(string ReferenceId, string Email);
