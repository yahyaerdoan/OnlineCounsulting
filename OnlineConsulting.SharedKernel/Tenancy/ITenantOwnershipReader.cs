namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read access to a tenant's recorded owner - lets Identity's role-change/removal
/// commands check "is this user the tenant's owner" without referencing the Tenancy module's Domain/Application
/// types, matching the project's cross-module convention (plain ids/shared-kernel interfaces only, see
/// IFeatureFlagReader/IFeatureFlagWriter and ITenantStatusReader for the same pattern applied elsewhere).</summary>
public interface ITenantOwnershipReader
{
    /// <summary>True only when the tenant exists and its OwnerUserId equals userId. An unknown/missing
    /// tenantId, or a tenant with no recorded owner, returns false.</summary>
    Task<bool> IsOwnerAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
}
