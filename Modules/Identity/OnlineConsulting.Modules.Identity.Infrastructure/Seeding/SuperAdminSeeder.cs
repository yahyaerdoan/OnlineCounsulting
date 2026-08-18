using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Seeding;

public static class SuperAdminSeeder
{
    /// <summary>Ensures the platform-owner SuperAdmin account exists, reading credentials from Seed:SuperAdmin config. No-ops when the email/password aren't configured.</summary>
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var email = configuration["Seed:SuperAdmin:Email"];
        var password = configuration["Seed:SuperAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var user = new User
        {
            UserName = email,
            Email = email,
            FirstName = "Super",
            LastName = "Admin",
            TenantId = TenantDefaults.DefaultTenantId,
            IsActive = true,
            EmailConfirmed = true,
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            return;

        await userManager.AddToRoleAsync(user, GlobalOperationClaims.SuperAdmin);
    }
}
