using Microsoft.Extensions.Caching.Memory;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Web.Infrastructure.Hosting;

/// <summary>Sends a business site's other addresses (its platform subdomain once a custom domain is live) to its canonical origin with a 301,
/// keeping the path and query. The origin comes from the Api per host, cached; any lookup failure just serves the page.</summary>
public class CanonicalHostMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, IMemoryCache cache)
{
    private const string CacheKeyPrefix = "canonical-host:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    // Blazor's circuit and framework files stay on the host they started on.
    private static readonly PathString[] PassThroughPaths = ["/_framework", "/_blazor", "/_content"];

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;

        // Page loads only; form posts stay where they started.
        if (!(HttpMethods.IsGet(request.Method) || HttpMethods.IsHead(request.Method)) || PassThroughPaths.Any(request.Path.StartsWithSegments))
        {
            await next(context);
            return;
        }

        var canonical = await GetCanonicalOriginAsync(request.Host.Value ?? string.Empty, context.RequestAborted);

        if (canonical is not null && !string.Equals(canonical.Authority, request.Host.Value, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.Redirect($"{canonical.GetLeftPart(UriPartial.Authority)}{request.PathBase}{request.Path}{request.QueryString}", permanent: true);
            return;
        }

        await next(context);
    }

    /// <summary>Null when the host is the platform, unknown, or the Api can't be reached.</summary>
    private async Task<Uri?> GetCanonicalOriginAsync(string host, CancellationToken cancellationToken) =>
        await cache.GetOrCreateAsync(CacheKeyPrefix + host, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;

            var client = new ApiClient(httpClientFactory.CreateClient(ApiHttpClientNames.Anonymous));
            var result = await client.GetAsync<TenantBrandingResponse>(ApiRoutes.Tenancy.Branding, cancellationToken);

            return result is { IsSuccessful: true, ResultData: { IsPlatform: false, SiteUrl: { } siteUrl } } && Uri.TryCreate(siteUrl, UriKind.Absolute, out var origin)
                ? origin
                : null;
        });
}
