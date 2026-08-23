using System.Globalization;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;

namespace OnlineConsulting.Maui.Web.Infrastructure.Auth;

/// <summary>Seeds from the cookie's claims, then refreshes in-memory - can't rewrite the cookie
/// mid-circuit (no live HTTP response to set it on).</summary>
public class ServerAccessTokenProvider(IHttpContextAccessor httpContextAccessor) : IAccessTokenProvider
{
    private TokenSet? _current;

    public Task<TokenSet?> GetTokenSetAsync()
    {
        if (_current is not null)
        {
            return Task.FromResult<TokenSet?>(_current);
        }

        var user = httpContextAccessor.HttpContext?.User;
        var accessToken = user?.FindFirst(AuthClaimTypes.AccessToken)?.Value;
        var refreshToken = user?.FindFirst(AuthClaimTypes.RefreshToken)?.Value;
        var expiresAtClaim = user?.FindFirst(AuthClaimTypes.AccessTokenExpiresAt)?.Value;

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(expiresAtClaim))
        {
            return Task.FromResult<TokenSet?>(null);
        }

        var expiresAt = DateTime.Parse(expiresAtClaim, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        _current = new TokenSet(accessToken, refreshToken, expiresAt);
        return Task.FromResult<TokenSet?>(_current);
    }

    public Task SetTokenSetAsync(TokenSet tokens)
    {
        _current = tokens;
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        _current = null;
        return Task.CompletedTask;
    }
}
