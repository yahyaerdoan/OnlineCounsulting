using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace OnlineConsulting.Maui.Web.Infrastructure.Auth;

/// <summary>Keeps the sign-in cookie's role/IsSuperAdmin claims in sync with the Api's refresh-token
/// flow. RefreshTokenCommand already re-reads the user's roles from the database on every refresh -
/// without this, that fresh access token gets used for Api calls but the cookie's own claims (what
/// [Authorize(Roles=...)] and IsSuperAdmin() actually check) stay frozen at login-time values until
/// the user manually signs out and back in.</summary>
public class CookiePrincipalRefresher(IHttpClientFactory httpClientFactory)
{
    private static readonly TimeSpan RefreshBuffer = TimeSpan.FromSeconds(60);

    public async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        var principal = context.Principal;
        var accessToken = principal?.FindFirst(AuthClaimTypes.AccessToken)?.Value;
        var refreshToken = principal?.FindFirst(AuthClaimTypes.RefreshToken)?.Value;
        var expiresAtClaim = principal?.FindFirst(AuthClaimTypes.AccessTokenExpiresAt)?.Value;

        if (accessToken is null || refreshToken is null || expiresAtClaim is null)
        {
            return;
        }

        var expiresAt = DateTime.Parse(expiresAtClaim, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        if (!new TokenSet(accessToken, refreshToken, expiresAt).IsNearExpiry(RefreshBuffer))
        {
            return;
        }

        var refreshClient = httpClientFactory.CreateClient(ApiHttpClientNames.Anonymous);
        var refreshApi = new ApiClient(refreshClient);
        var refreshResult = await refreshApi.PostAsync<AuthTokensResponse>(ApiRoutes.Auth.Refresh, new { accessToken, refreshToken });

        if (!refreshResult.IsSuccessful || refreshResult.ResultData is null)
        {
            await RejectAsync(context);
            return;
        }

        var meClient = httpClientFactory.CreateClient(ApiHttpClientNames.Anonymous);
        meClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.ResultData.AccessToken);
        var userResult = await new ApiClient(meClient).GetAsync<CurrentUserResponse>(ApiRoutes.Users.Me);

        if (!userResult.IsSuccessful || userResult.ResultData is null)
        {
            await RejectAsync(context);
            return;
        }

        List<Claim> claims =
        [
            .. UserClaimsFactory.BuildBaseClaims(userResult.ResultData),
            new Claim(AuthClaimTypes.AccessToken, refreshResult.ResultData.AccessToken),
            new Claim(AuthClaimTypes.RefreshToken, refreshResult.ResultData.RefreshToken),
            new Claim(AuthClaimTypes.AccessTokenExpiresAt, refreshResult.ResultData.AccessTokenExpiresAt.ToString("O", CultureInfo.InvariantCulture)),
        ];

        context.ReplacePrincipal(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        context.ShouldRenew = true;
    }

    private static async Task RejectAsync(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
