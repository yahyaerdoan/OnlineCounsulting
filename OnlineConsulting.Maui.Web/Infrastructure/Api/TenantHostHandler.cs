namespace OnlineConsulting.Maui.Web.Infrastructure.Api;

/// <summary>Tells the Api which business's site the browser is on (e.g. acme.comfortpro.com), so anonymous calls such as login, register and public pages run against that tenant.</summary>
public class TenantHostHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    public const string HeaderName = "X-Tenant-Host";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Already forwarded by the caller (e.g. a circuit); skip the stale HttpContext.
        if (request.Headers.Contains(HeaderName))
        {
            return base.SendAsync(request, cancellationToken);
        }

        var host = httpContextAccessor.HttpContext?.Request.Host.Host;
        if (!string.IsNullOrEmpty(host))
        {
            request.Headers.Add(HeaderName, host);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
