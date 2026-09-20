namespace OnlineConsulting.SharedKernel.Tenancy;

/// <summary>Cross-module read access to a tenant's recorded owner, so Identity's role-change/removal commands can check ownership without referencing Tenancy's Domain/Application types.</summary>
public interface ITenantOwnershipReader
{
    /// <summary>True only when the tenant exists and its OwnerUserId equals userId. An unknown/missing
    /// tenantId, or a tenant with no recorded owner, returns false.</summary>
    Task<bool> IsOwnerAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
}
