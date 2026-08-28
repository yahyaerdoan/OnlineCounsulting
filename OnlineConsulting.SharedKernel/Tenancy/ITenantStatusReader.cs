namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read access to a tenant's access state, for TenantStatusCheckBehavior.</summary>
public interface ITenantStatusReader
{
    /// <summary>True unless the tenant is Active or PastDue (grace period); false for an unknown tenantId.</summary>
    Task<bool> IsBlockedAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
