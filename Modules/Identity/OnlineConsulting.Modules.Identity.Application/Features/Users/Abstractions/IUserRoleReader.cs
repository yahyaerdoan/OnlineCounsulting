namespace OnlineConsulting.Modules.Identity.Application.Features.Users.Abstractions;

/// <summary>Role membership read straight from the role assignments, independent of the ambient tenant. UserManager's GetUsersInRoleAsync only sees
/// the current tenant's users, which is wrong for a Super Admin's cross-tenant screens and for "last admin" checks run under a TenantScope.</summary>
public interface IUserRoleReader
{
    /// <summary>Ids of every user holding the role, across tenants; narrow the result with a tenant-filtered user query.</summary>
    Task<IReadOnlyList<Guid>> GetUserIdsInRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>Role names per user for the given users.</summary>
    Task<ILookup<Guid, string>> GetRoleNamesAsync(IReadOnlyCollection<Guid> userIds, CancellationToken cancellationToken = default);

    /// <summary>True when another active user holds the role, within <paramref name="tenantId"/> or across all tenants when it is null.</summary>
    Task<bool> AnyOtherActiveUserInRoleAsync(string roleName, Guid excludedUserId, Guid? tenantId, CancellationToken cancellationToken = default);
}
