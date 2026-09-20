namespace OnlineConsulting.SharedKernel.Identity;

/// <summary>Cross-module read access to whether a tenant ever had a user account, so Tenancy's orphaned-tenant cleanup can tell an abandoned signup from a tenant already claimed by a user.</summary>
public interface IUserExistenceReader
{
    /// <summary>True if at least one non-deleted User row exists for the given TenantId.</summary>
    Task<bool> AnyUserExistsForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
