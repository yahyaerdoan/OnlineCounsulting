using Microsoft.AspNetCore.Components.Authorization;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using System.Security.Claims;

namespace OnlineConsulting.Maui.Infrastructure.Auth;

/// <summary>MAUI has no cookie/circuit to carry identity, so auth state is cached in memory, restored from the persisted token on first use.</summary>
public class MauiAuthenticationStateProvider(IApiClient apiClient, SecureStorageAccessTokenProvider tokenProvider) : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    private ClaimsPrincipal? _cachedUser;

    /// <summary>Returns the cached auth state, restoring it from the persisted token on first use; a failed restore is not cached, so the next call retries instead of locking the user out until app restart.</summary>
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

    private static ClaimsPrincipal BuildPrincipal(CurrentUserResponse user) => new(new ClaimsIdentity(UserClaimsFactory.BuildBaseClaims(user), "ApiToken"));
}
