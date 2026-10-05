using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Refreshes a near-expiry token set, single-flight (concurrent callers share one call).</summary>
public class TokenRefresher(IHttpClientFactory httpClientFactory, IAccessTokenProvider tokenProvider)
{
    private static readonly TimeSpan RefreshBuffer = TimeSpan.FromSeconds(30);

    private readonly SemaphoreSlim _lock = new(1, 1);
    private Task<TokenSet?>? _inFlight;

    /// <summary>The signed-in user's access token, refreshed first when it is about to expire; null when signed out.</summary>
    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var tokens = await tokenProvider.GetTokenSetAsync();
        if (tokens is not null && tokens.IsNearExpiry(RefreshBuffer))
        {
            tokens = await RefreshAsync(tokens, cancellationToken) ?? tokens;
        }

        return tokens?.AccessToken;
    }

    /// <returns>The refreshed token set, or null if the exchange failed.</returns>
    public async Task<TokenSet?> RefreshAsync(TokenSet current, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            _inFlight ??= ExchangeAsync(current, cancellationToken);
            return await _inFlight;
        }
        finally
        {
            _inFlight = null;
            _ = _lock.Release();
        }
    }

    private async Task<TokenSet?> ExchangeAsync(TokenSet current, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(ApiHttpClientNames.Anonymous);
        var api = new ApiClient(client);

        var result = await api.PostAsync<AuthTokensResponse>(ApiRoutes.Auth.Refresh,
            new { accessToken = current.AccessToken, refreshToken = current.RefreshToken }, cancellationToken);

        if (!result.IsSuccessful || result.ResultData is null)
        {
            return null;
        }

        var refreshed = new TokenSet(result.ResultData.AccessToken, result.ResultData.RefreshToken, result.ResultData.AccessTokenExpiresAt);

        await tokenProvider.SetTokenSetAsync(refreshed);

        return refreshed;
    }
}
