using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;
using OnlineConsulting.Maui.Shared.Infrastructure.Auth;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace OnlineConsulting.Maui.Web.Infrastructure.Auth;

/// <summary>Keeps the sign-in cookie's role/IsSuperAdmin claims in sync with the Api's refresh-token flow, so they don't stay frozen at login-time values.</summary>
public class CookiePrincipalRefresher(IHttpClientFactory httpClientFactory, IRotatedTokenStore rotatedTokens)
{
    private static readonly TimeSpan RefreshBuffer = TimeSpan.FromSeconds(60);

    /// <summary>Runs on each cookie validation; refreshes tokens and claims if near expiry, rejects the principal on failure.</summary>
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
        var tokens = new TokenSet(accessToken, refreshToken, expiresAt);

        // A circuit may have rotated the cookie's token; take its newer set instead of presenting a dead one.
        if (rotatedTokens.FindLatest(refreshToken) is { } newer)
        {
            tokens = newer;
            if (!tokens.IsNearExpiry(RefreshBuffer))
            {
                context.ReplacePrincipal(WithTokens(context.Principal, tokens));
                context.ShouldRenew = true;
                return;
            }
        }

        if (!tokens.IsNearExpiry(RefreshBuffer))
        {
            return;
        }

        var refreshClient = httpClientFactory.CreateClient(ApiHttpClientNames.Anonymous);
        var refreshApi = new ApiClient(refreshClient);
        var refreshResult = await refreshApi.PostAsync<AuthTokensResponse>(ApiRoutes.Auth.Refresh, new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken });

        if (!refreshResult.IsSuccessful || refreshResult.ResultData is null)
        {
            await RejectAsync(context);

            return;
        }

        var refreshed = new TokenSet(refreshResult.ResultData.AccessToken, refreshResult.ResultData.RefreshToken, refreshResult.ResultData.AccessTokenExpiresAt);
        rotatedTokens.Record(tokens.RefreshToken, refreshed);

        var meClient = httpClientFactory.CreateClient(ApiHttpClientNames.Anonymous);
        meClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshResult.ResultData.AccessToken);
        var userResult = await new ApiClient(meClient).GetAsync<CurrentUserResponse>(ApiRoutes.Users.Me);

        if (!userResult.IsSuccessful || userResult.ResultData is null)
        {
            await RejectAsync(context);

            return;
        }

        List<Claim> claims = [.. UserClaimsFactory.BuildBaseClaims(userResult.ResultData), .. TokenClaims(refreshed)];

        context.ReplacePrincipal(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        context.ShouldRenew = true;
    }

    private static Claim[] TokenClaims(TokenSet tokens) =>
    [
        new Claim(AuthClaimTypes.AccessToken, tokens.AccessToken),
        new Claim(AuthClaimTypes.RefreshToken, tokens.RefreshToken),
        new Claim(AuthClaimTypes.AccessTokenExpiresAt, tokens.AccessTokenExpiresAt.ToString("O", CultureInfo.InvariantCulture)),
    ];

    /// <summary>Same user claims, newer tokens.</summary>
    private static ClaimsPrincipal WithTokens(ClaimsPrincipal? principal, TokenSet tokens)
    {
        string[] tokenClaimTypes = [AuthClaimTypes.AccessToken, AuthClaimTypes.RefreshToken, AuthClaimTypes.AccessTokenExpiresAt];
        var userClaims = principal?.Claims.Where(c => !tokenClaimTypes.Contains(c.Type)) ?? [];

        return new ClaimsPrincipal(new ClaimsIdentity([.. userClaims, .. TokenClaims(tokens)], CookieAuthenticationDefaults.AuthenticationScheme));
    }

    private static async Task RejectAsync(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();

        await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
