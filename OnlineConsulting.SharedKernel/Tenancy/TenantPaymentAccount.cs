namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>ConnectedAccountId is the id to pass as the gateway's connected-account target; IsReadyForCharges mirrors Tenant.IsReadyToAcceptPayments.</summary>
public record TenantPaymentAccount(string? ConnectedAccountId, bool IsReadyForCharges);
