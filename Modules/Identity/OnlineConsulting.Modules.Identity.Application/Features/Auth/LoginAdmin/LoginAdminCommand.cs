using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.LoginCookie;
using OnlineConsulting.Modules.Identity.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.LoginAdmin;

public record LoginAdminCommand(string UserNameOrEmail, string Password, bool RememberMe) : IRequest<OperationResult>;

public class LoginAdminHandler(UserManager<User> userManager, SignInManager<User> signInManager) : IRequestHandler<LoginAdminCommand, OperationResult>
{
    private static readonly string[] AdminRoles = [GeneralOperationClaims.Admin, "SuperAdmin", "Super Admin"];

    public async Task<OperationResult> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.UserNameOrEmail)
            ?? await userManager.Users.FirstOrDefaultAsync(u => u.Email == request.UserNameOrEmail, cancellationToken);

        if (user is null)
            return Result.BadRequest("Invalid credentials. Please check your username and password.");

        var roles = await userManager.GetRolesAsync(user);

        if (!roles.Any(r => AdminRoles.Contains(r)))
            return Result.BadRequest("Invalid credentials. Please check your username and password.");

        var signInResult = await signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
            return await LoginCookieHandler.LockoutMessageAsync(userManager, user);

        if (!signInResult.Succeeded)
            return Result.BadRequest("Invalid credentials. Please check your username and password.");

        return Result.Success($"Welcome back, {user.FirstName} {user.LastName}!");
    }
}
