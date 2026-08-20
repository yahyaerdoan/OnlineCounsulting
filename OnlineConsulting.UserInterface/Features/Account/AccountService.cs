using Core.SecurityLayer.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OnlineConsulting.UserInterface.Common;
using OnlineConsulting.UserInterface.Infrastructure.Api;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Security.Claims;

namespace OnlineConsulting.UserInterface.Features.Account;

public class AccountService(IApiClient apiClient, IHttpContextAccessor httpContextAccessor) : IAccountService
{
    public async Task<OperationResult> RegisterAsync(string firstName, string lastName, string userName, string email, string password, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.PostAsync("/api/auth/register", new { firstName, lastName, userName, email, password }, cancellationToken);
        return result.ToOperationResult();
    }

    public async Task<LoginResult> LoginAsync(string userNameOrEmail, string password, bool rememberMe, CancellationToken cancellationToken = default)
    {
        var user = await AuthenticateAsync(userNameOrEmail, password, cancellationToken);
        if (user is null)
        {
            return new LoginResult(Result.BadRequest("Invalid credentials. Please check your username and password."), IsAdmin: false);
        }

        await SignInLocallyAsync(user, rememberMe);
        var isAdmin = user.Permissions.Count > 0;
        return new LoginResult(Result.Success($"Welcome back, {user.FirstName} {user.LastName}!"), isAdmin);
    }

    public async Task<OperationResult> LogoutAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = GetHttpContext();
        await httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        httpContext.Session.Remove(ApiSessionKeys.AccessToken);
        return Result.Success("You've been logged out. See you again soon!");
    }

    /// <summary>Verifies credentials against the Api (the only place that ever checks a password) and stores the
    /// resulting access token in Session. Returns null on any failure (bad credentials, or the Api rejecting the
    /// just-issued token on the immediate follow-up call).</summary>
    private async Task<CurrentUserResponse?> AuthenticateAsync(string userNameOrEmail, string password, CancellationToken cancellationToken)
    {
        var httpContext = GetHttpContext();

        var tokenResult = await apiClient.PostAsync<AuthTokensResponseDto>("/api/auth/login", new { userNameOrEmail, password }, cancellationToken);
        if (!tokenResult.IsSuccessful || tokenResult.ResultData is null)
        {
            return null;
        }

        httpContext.Session.SetString(ApiSessionKeys.AccessToken, tokenResult.ResultData.AccessToken);

        var userResult = await apiClient.GetAsync<CurrentUserResponse>("/api/users/me", cancellationToken);
        if (userResult.IsSuccessful && userResult.ResultData is not null)
        {
            return userResult.ResultData;
        }

        httpContext.Session.Remove(ApiSessionKeys.AccessToken);
        return null;
    }

    private async Task SignInLocallyAsync(CurrentUserResponse user, bool rememberMe)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            .. user.Roles.Select(role => new Claim(ClaimTypes.Role, role)),
            .. user.Permissions.Select(permission => new Claim(PermissionClaimTypes.Type, permission)),
        ];

        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        await GetHttpContext().SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = rememberMe });
    }

    private HttpContext GetHttpContext() =>
        httpContextAccessor.HttpContext ?? throw new InvalidOperationException($"{nameof(AccountService)} requires an active HTTP request.");
}
