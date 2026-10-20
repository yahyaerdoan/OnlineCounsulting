using System.Security.Claims;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Web.Infrastructure.Api;

/// <summary>Host, guest id and user copied while the request is live; a circuit outlives its HttpContext.</summary>
public sealed class BrowserRequestContext : IBrowserRequestForwarder
{
    private const string GuestCookieName = "guest_id";
    private static readonly TimeSpan GuestCookieLifetime = TimeSpan.FromDays(90);

    private string? _host;
    private string? _guestId;

    // Set only inside an HTTP request scope, where the cookie can still be written.
    private HttpResponse? _response;

    /// <summary>Signed-in user as of the last capture.</summary>
    public ClaimsPrincipal? User { get; private set; }

    public void Capture(HttpContext context, bool canWriteCookies)
    {
        _host = context.Request.Host.Host;
        _guestId = context.Request.Cookies[GuestCookieName];
        _response = canWriteCookies ? context.Response : null;
        User = context.User;
    }

    public void Apply(HttpRequestMessage request)
    {
        if (!string.IsNullOrEmpty(_host))
        {
            _ = request.Headers.Remove(TenantHostHandler.HeaderName);
            request.Headers.Add(TenantHostHandler.HeaderName, _host);
        }

        if (!string.IsNullOrEmpty(_guestId) && !request.Headers.Contains("Cookie"))
        {
            request.Headers.Add("Cookie", $"{GuestCookieName}={_guestId}");
        }
    }

    public void Relay(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var setCookies) || setCookies.Select(ExtractGuestId).FirstOrDefault(v => v is not null) is not { } guestId)
        {
            return;
        }

        _guestId = guestId;

        if (_response is { HasStarted: false })
        {
            _response.Cookies.Append(GuestCookieName, guestId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.Add(GuestCookieLifetime),
            });
        }
    }

    private static string? ExtractGuestId(string setCookieHeader)
    {
        const string prefix = $"{GuestCookieName}=";
        if (!setCookieHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var afterName = setCookieHeader[prefix.Length..];
        var separatorIndex = afterName.IndexOf(';');
        return separatorIndex >= 0 ? afterName[..separatorIndex] : afterName;
    }
}
