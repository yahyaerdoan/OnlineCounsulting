namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read access to a tenant's suspension state - lets TenantStatusCheckBehavior gate the whole request pipeline without referencing the Tenancy module's Domain/Application types, matching the project's cross-module convention (plain ids/shared-kernel interfaces only, see IFeatureFlagReader/IFeatureFlagWriter for the same pattern applied to feature flags).</summary>
public interface ITenantStatusReader
{
    /// <summary>True only when the tenant exists and its Status is Suspended. An unknown/missing tenantId returns false defensively, so ids that predate the Tenancy module never get blocked.</summary>
    Task<bool> IsSuspendedAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
