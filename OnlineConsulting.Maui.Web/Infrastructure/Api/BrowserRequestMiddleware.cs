namespace OnlineConsulting.Maui.Web.Infrastructure.Api;

/// <summary>Fills this request's <see cref="BrowserRequestContext"/>; runs after the edge host rewrite and authentication.</summary>
public class BrowserRequestMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context, BrowserRequestContext browserRequest)
    {
        browserRequest.Capture(context, canWriteCookies: true);
        return next(context);
    }
}
