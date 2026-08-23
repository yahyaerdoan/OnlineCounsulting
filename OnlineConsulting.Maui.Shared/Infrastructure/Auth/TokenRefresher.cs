using System.Net.Http;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Auth;

/// <summary>Refreshes a near-expiry token set, single-flight (concurrent callers share one call).</summary>
public class TokenRefresher(IHttpClientFactory httpClientFactory, IAccessTokenProvider tokenProvider)
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private Task<TokenSet?>? _inFlight;

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
            _lock.Release();
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
