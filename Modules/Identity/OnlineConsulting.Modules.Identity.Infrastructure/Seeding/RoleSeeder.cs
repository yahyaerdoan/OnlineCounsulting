using Core.SecurityLayer.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using System.Security.Claims;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Seeding;

public static class RoleSeeder
{
    private static readonly string[] Roles = [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin, GlobalOperationClaims.Member];

    /// <summary>Ensures the built-in roles exist and hold their baseline permission claims.</summary>
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        foreach (var roleName in Roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                _ = await roleManager.CreateAsync(new Role { Name = roleName });
            }
        }

        await GrantPermissionAsync(roleManager, GeneralOperationClaims.Admin, PermissionClaimTypes.FullAccess);
        await GrantPermissionAsync(roleManager, GlobalOperationClaims.SuperAdmin, GlobalOperationClaims.SuperAdmin);
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
}
