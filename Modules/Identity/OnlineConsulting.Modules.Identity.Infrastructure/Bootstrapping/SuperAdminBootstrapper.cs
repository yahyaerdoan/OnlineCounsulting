using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Identity.Application.Common;
using OnlineConsulting.Modules.Identity.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Bootstrapping;

public static class SuperAdminBootstrapper
{
    /// <summary>Ensures the platform-owner SuperAdmin account exists, reading credentials from Bootstrap:SuperAdmin config. No-ops when the email/password aren't configured.</summary>
    public static async Task EnsureAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<SuperAdminBootstrapOptions>>().Value;
        var email = options.Email;
        var password = options.Password;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        if (await userManager.FindByEmailInTenantAsync(email, TenantDefaults.DefaultTenantId) is not null)
        {
            return;
        }

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
        {
            return;
        }

        _ = await userManager.AddToRoleAsync(user, GlobalOperationClaims.SuperAdmin);
    }
}
