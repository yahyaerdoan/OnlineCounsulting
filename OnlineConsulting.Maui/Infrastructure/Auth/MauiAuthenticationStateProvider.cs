using Microsoft.AspNetCore.Components.Authorization;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using System.Security.Claims;

namespace OnlineConsulting.Maui.Infrastructure.Auth;

/// <summary>MAUI has no cookie/circuit to carry identity, so auth state lives here: in memory once
/// signed in, restored from the persisted token by validating it against ApiRoutes.Users.Me the
/// first time anything asks for the auth state (typically right after app launch).</summary>
public class MauiAuthenticationStateProvider(IApiClient apiClient, SecureStorageAccessTokenProvider tokenProvider) : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    private ClaimsPrincipal? _cachedUser;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_cachedUser is not null)
        {
            return new AuthenticationState(_cachedUser);
        }

        var tokens = await tokenProvider.GetTokenSetAsync();
        if (tokens is null)
        {
            _cachedUser = Anonymous;
            return new AuthenticationState(_cachedUser);
        }

        // A failed restore (network down, expired token, ...) intentionally isn't cached, so the
        // very next check (e.g. after the user retries) gets a fresh attempt instead of being
        // locked out until the app restarts. ApiClient already turns a transport-level failure into
        // a plain unsuccessful envelope, so there is nothing to catch here.
        var result = await apiClient.GetAsync<CurrentUserResponse>(ApiRoutes.Users.Me);
        if (result is { IsSuccessful: true, ResultData: not null })
        {
            _cachedUser = BuildPrincipal(result.ResultData);
            return new AuthenticationState(_cachedUser);
        }

        return new AuthenticationState(Anonymous);
    }

    public void SignIn(CurrentUserResponse user)
    {
        _cachedUser = BuildPrincipal(user);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
    }

    public void SignOut()
    {
        _cachedUser = Anonymous;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedUser)));
    }

    private static ClaimsPrincipal BuildPrincipal(CurrentUserResponse user) =>
        new(new ClaimsIdentity(UserClaimsFactory.BuildBaseClaims(user), "ApiToken"));
}
