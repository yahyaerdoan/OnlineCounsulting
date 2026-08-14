using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Constants;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Rules;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.LoginCookie;

public record LoginCookieCommand(string UserNameOrEmail, string Password, bool RememberMe) : IRequest<OperationResult>;

public class LoginCookieHandler(UserManager<User> userManager, SignInManager<User> signInManager) : IRequestHandler<LoginCookieCommand, OperationResult>
{
    public async Task<OperationResult> Handle(LoginCookieCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.UserNameOrEmail)
            ?? await userManager.Users.FirstOrDefaultAsync(u => u.Email == request.UserNameOrEmail, cancellationToken);

        if (user is null)
            return AuthBusinessRules.InvalidCredentials();

        var signInResult = await signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
            return await LockoutMessageAsync(userManager, user);

        if (!signInResult.Succeeded)
            return AuthBusinessRules.InvalidCredentials();

        return Result.Success($"Welcome back, {user.FirstName} {user.LastName}!");
    }

    internal static async Task<OperationResult> LockoutMessageAsync(UserManager<User> userManager, User user)
    {
        var lockoutEnd = await userManager.GetLockoutEndDateAsync(user);
        var remaining = lockoutEnd?.UtcDateTime - DateTime.UtcNow;

        var message = remaining is { TotalSeconds: > 0 }
            ? $"Your account is locked. Try again in {remaining.Value.Minutes} minute(s)."
            : AuthMessages.AccountLocked;

        return Result.Forbidden(message);
    }
}
