using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Application.Common;

/// <summary>Replaces Identity's global RequireUniqueEmail: the same person may hold separate accounts with different tenants, but not two in one tenant.
/// The (TenantId, NormalizedEmail) unique index backs this up against races.</summary>
public class TenantEmailUserValidator : IUserValidator<User>
{
    public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        var email = await manager.GetEmailAsync(user);
        if (string.IsNullOrWhiteSpace(email))
        {
            return IdentityResult.Failed(manager.ErrorDescriber.InvalidEmail(email));
        }

        var normalizedEmail = manager.NormalizeEmail(email);
        var taken = await manager.Users.AnyAsync(u => u.TenantId == user.TenantId && u.NormalizedEmail == normalizedEmail && u.Id != user.Id);

        return taken ? IdentityResult.Failed(manager.ErrorDescriber.DuplicateEmail(email)) : IdentityResult.Success;
    }
}
