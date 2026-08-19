namespace OnlineConsulting.SharedKernel.Identity;

/// <summary>Cross-module read access to whether any admin/user account was ever created for a tenant - lets Tenancy's orphaned-tenant cleanup job tell a genuinely abandoned signup (no User row, safe to reap) apart from a tenant that is merely stuck mid-billing but was already claimed by a user, without referencing the Identity module's Domain/Application types, matching the project's cross-module convention (see ITenantStatusReader for the same pattern applied to tenant suspension).</summary>
public interface IUserExistenceReader
{
    /// <summary>True if at least one non-deleted User row exists for the given TenantId.</summary>
    Task<bool> AnyUserExistsForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
