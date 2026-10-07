namespace OnlineConsulting.Maui.Web.Infrastructure.Api;

/// <summary>Tells the Api which business's site the browser is on (e.g. acme.comfortpro.com), so anonymous calls such as login, register and public pages run against that tenant.</summary>
public class TenantHostHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private const string HeaderName = "X-Tenant-Host";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var host = httpContextAccessor.HttpContext?.Request.Host.Host;
        if (!string.IsNullOrEmpty(host))
        {
            _ = request.Headers.Remove(HeaderName);
            request.Headers.Add(HeaderName, host);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
