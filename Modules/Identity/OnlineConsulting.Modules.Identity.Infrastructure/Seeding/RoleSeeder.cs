using Core.SecurityLayer.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using System.Security.Claims;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Seeding;

public static class RoleSeeder
{
    private static readonly string[] _roles = [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, GlobalOperationClaims.Member, GlobalOperationClaims.User];

    /// <summary>Ensures the built-in roles exist and hold their baseline permission claims.</summary>
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        foreach (var roleName in _roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                _ = await roleManager.CreateAsync(new Role { Name = roleName });
            }
        }

        // No need to also grant SuperAdmin its own role name as a permission claim - role membership already
        // satisfies every Roles[] check, and FullAccess already bypasses everything else.
        await GrantPermissionAsync(roleManager, GlobalOperationClaims.SuperAdmin, PermissionClaimTypes.FullAccess);

        // Admin gets each module's own default grant (see IDefaultAdminPermissions) instead of a coarse
        // bypass - every module registers its own claims, so Identity never references other modules'
        // Application layers. A module with no registration (Tenancy, Identity's own Roles claims) stays
        // out of Admin's reach - see GlobalOperationClaims.SuperAdmin's doc comment for why.
        foreach (var permission in scope.ServiceProvider.GetServices<IDefaultAdminPermissions>().SelectMany(p => p.Permissions).Distinct())
        {
            await GrantPermissionAsync(roleManager, GeneralOperationClaims.Admin, permission);
        }

        // One-time cleanup: Admin used to hold a coarse bypass claim - no longer granted, so drop any leftover.
        await RevokePermissionAsync(roleManager, GeneralOperationClaims.Admin, PermissionClaimTypes.FullAccess);
        await RevokePermissionAsync(roleManager, GeneralOperationClaims.Admin, PermissionClaimTypes.TenantFullAccess);
    }

    private static async Task GrantPermissionAsync(RoleManager<Role> roleManager, string roleName, string permission)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            return;
        }

        var existingClaims = await roleManager.GetClaimsAsync(role);
        if (!existingClaims.Any(c => c.Type == PermissionClaimTypes.Type && c.Value == permission))
        {
            _ = await roleManager.AddClaimAsync(role, new Claim(PermissionClaimTypes.Type, permission));
        }
    }

    private static async Task RevokePermissionAsync(RoleManager<Role> roleManager, string roleName, string permission)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            return;
        }

        var staleClaim = (await roleManager.GetClaimsAsync(role)).FirstOrDefault(c => c.Type == PermissionClaimTypes.Type && c.Value == permission);
        if (staleClaim is not null)
        {
            _ = await roleManager.RemoveClaimAsync(role, staleClaim);
        }
    }
}
