using Microsoft.AspNetCore.Identity;
using OnlineConsulting.Modules.Identity.Domain;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.Abstractions;

public interface IPasswordChecker
{
    /// <summary>Checks a sign-in password and counts the failure toward lockout; it never signs anyone in (tokens are issued separately).</summary>
    Task<SignInResult> CheckAsync(User user, string password);
}
