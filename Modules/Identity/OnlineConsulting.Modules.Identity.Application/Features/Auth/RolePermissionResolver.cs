using Core.SecurityLayer.Constants;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;

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
                continue;

            var claims = await roleManager.GetClaimsAsync(role);
            permissions.AddRange(claims.Where(c => c.Type == PermissionClaimTypes.Type).Select(c => c.Value));
        }

        return [.. permissions.Distinct()];
    }
}
