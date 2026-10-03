using Microsoft.AspNetCore.Components.Authorization;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using OnlineConsulting.Maui.Shared.Infrastructure.LiveUpdates;
using System.Security.Claims;

namespace OnlineConsulting.Maui.Infrastructure.Auth;

/// <summary>MAUI has no cookie/circuit to carry identity, so auth state is cached in memory, restored from the persisted token on first use,
/// and re-read from /me when the app resumes or the profile changes (name/roles edited on another device).</summary>
public sealed class MauiAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    private readonly IApiClient apiClient;
    private readonly SecureStorageAccessTokenProvider tokenProvider;
    private readonly IDisposable _subscription;
    private ClaimsPrincipal? _cachedUser;

    public MauiAuthenticationStateProvider(IApiClient apiClient, SecureStorageAccessTokenProvider tokenProvider, DataChangeNotifier notifier)
    {
        this.apiClient = apiClient;
        this.tokenProvider = tokenProvider;
        _subscription = notifier.Subscribe(RefreshSignedInUserAsync, DataTopic.Profile);
    }

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

    /// <summary>Re-reads /me for a signed-in user and notifies only when name, email or roles actually changed; failures keep the cached user.</summary>
    private async Task RefreshSignedInUserAsync()
    {
        if (_cachedUser?.Identity?.IsAuthenticated is not true)
        {
            return;
        }

        var result = await apiClient.GetAsync<CurrentUserResponse>(ApiRoutes.Users.Me);
        if (result is not { IsSuccessful: true, ResultData: { } user })
        {
            return;
        }

        var refreshed = BuildPrincipal(user);
        if (Fingerprint(refreshed) == Fingerprint(_cachedUser))
        {
            return;
        }

        _cachedUser = refreshed;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(refreshed)));
    }

    private static string Fingerprint(ClaimsPrincipal principal) =>
        string.Join('|', principal.Claims.Select(c => $"{c.Type}={c.Value}").Order(StringComparer.Ordinal));

    private static ClaimsPrincipal BuildPrincipal(CurrentUserResponse user) => new(new ClaimsIdentity(UserClaimsFactory.BuildBaseClaims(user), "ApiToken"));

    public void Dispose() => _subscription.Dispose();
}
