using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.Maui.Shared.Infrastructure.Api;

/// <summary>Forwards the browser's guest_id cookie to the Api and relays back whatever it issues/refreshes.</summary>
public class GuestIdHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private const string CookieName = "guest_id";
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromDays(90);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null && httpContext.Request.Cookies.TryGetValue(CookieName, out var guestId) && !request.Headers.Contains("Cookie"))
        {
            request.Headers.Add("Cookie", $"{CookieName}={guestId}");
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (httpContext is not null && !httpContext.Response.HasStarted && response.Headers.TryGetValues("Set-Cookie", out var setCookies))
        {
            foreach (var setCookie in setCookies)
            {
                var value = ExtractGuestId(setCookie);
                if (value is not null)
                {
                    httpContext.Response.Cookies.Append(CookieName, value, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.Add(CookieLifetime),
                    });
                    break;
                }
            }
        }

        return response;
    }

    private static string? ExtractGuestId(string setCookieHeader)
    {
        const string prefix = $"{CookieName}=";
        if (!setCookieHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var afterName = setCookieHeader[prefix.Length..];
        var separatorIndex = afterName.IndexOf(';');
        return separatorIndex >= 0 ? afterName[..separatorIndex] : afterName;
    }
}
