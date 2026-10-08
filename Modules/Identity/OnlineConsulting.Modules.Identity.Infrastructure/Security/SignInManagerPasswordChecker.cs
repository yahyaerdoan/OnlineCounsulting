using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;
using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Infrastructure.Security;

/// <summary>ASP.NET Identity's password check with lockout, kept out of Application because SignInManager is an ASP.NET Core (HttpContext) type.</summary>
public class SignInManagerPasswordChecker(SignInManager<User> signInManager) : IPasswordChecker
{
    public Task<SignInResult> CheckAsync(User user, string password) =>
        signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
}
