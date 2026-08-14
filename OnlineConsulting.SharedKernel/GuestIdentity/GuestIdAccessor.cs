using Microsoft.AspNetCore.Http;

namespace OnlineConsulting.SharedKernel.GuestIdentity;

public class GuestIdAccessor(IHttpContextAccessor httpContextAccessor) : IGuestIdAccessor
{
    private const string CookieName = "guest_id";
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromDays(90);

    public Guid? TryGetGuestId()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null && httpContext.Request.Cookies.TryGetValue(CookieName, out var raw) && Guid.TryParse(raw, out var guestId))
            return guestId;

        return null;
    }

    public Guid GetOrCreateGuestId()
    {
        if (TryGetGuestId() is { } existing)
            return existing;

        var httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException($"{nameof(GetOrCreateGuestId)} requires an active HTTP request.");

        var guestId = Guid.NewGuid();
        httpContext.Response.Cookies.Append(CookieName, guestId.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.Add(CookieLifetime),
        });

        return guestId;
    }

    public void ClearGuestId()
    {
        var httpContext = httpContextAccessor.HttpContext;
        httpContext?.Response.Cookies.Delete(CookieName);
    }
}
