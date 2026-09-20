using Core.SecurityLayer.Authorization;
using Core.SecurityLayer.Constants;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth;

public static class RolePermissionResolver
{
    /// <summary>Returns the distinct permission claims granted across the given role names.</summary>
    public static async Task<List<string>> ResolvePermissionsAsync(RoleManager<Role> roleManager, IEnumerable<string> roleNames)
    {
        var permissions = new List<string>();

        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                continue;
            }

            var claims = await roleManager.GetClaimsAsync(role);
            permissions.AddRange(claims.Where(c => c.Type == PermissionClaimTypes.Type).Select(c => c.Value));
        }

        return [.. permissions.Distinct()];
    }

    /// <summary>Removes any permission the user has individually denied (PermissionOverrideClaimTypes.Deny) from
    /// their role-derived list - lets a tenant Owner narrow one team member without touching the shared role.
    /// Never removes a bypass claim (FullAccess/TenantFullAccess/SuperAdmin) - those aren't "permissions" in the
    /// catalog sense, and per-user denial only ever targets the actual catalog claims a role grants.</summary>
    public static async Task<List<string>> ApplyUserOverridesAsync(UserManager<User> userManager, User user, List<string> rolePermissions)
    {
        var deniedPermissions = (await userManager.GetClaimsAsync(user)).Where(c => c.Type == PermissionOverrideClaimTypes.Deny).Select(c => c.Value).ToHashSet();

        return deniedPermissions.Count == 0 ? rolePermissions : [.. rolePermissions.Where(p => !deniedPermissions.Contains(p))];
    }

    /// <summary>Replaces bypass claims (FullAccess/SuperAdmin) with the full permission catalog for display, so API
    /// responses show what a bypass actually grants instead of the opaque flag itself. Authorization checks never
    /// call this - they keep matching the raw bypass claim.</summary>
    public static List<string> ExpandForDisplay(List<string> permissions, IPermissionCatalog permissionCatalog) =>
        permissions.Contains(PermissionClaimTypes.FullAccess) || permissions.Contains(GlobalOperationClaims.SuperAdmin)
            ? [.. permissionCatalog.AllPermissions]
            : permissions;
}
