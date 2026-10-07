using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Hosting;

/// <summary>Builds "{scheme}://{slug}.{SubdomainRoot}{:port}" from the platform origin; the default tenant and unknown tenants get the platform origin.
/// Never derived from request headers, so a forged host cannot redirect emailed links.</summary>
public class TenantOriginReader(ITenantRepository tenantRepository, IMemoryCache cache, IOptions<TenantHostingOptions> options) : ITenantOriginReader
{
    public async Task<string> GetOriginAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        var platformOrigin = new Uri(settings.PlatformOrigin);

        if (tenantId == TenantDefaults.DefaultTenantId)
        {
            return platformOrigin.GetLeftPart(UriPartial.Authority);
        }

        var slug = await cache.GetOrCreateAsync($"tenant-slug:{tenantId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = settings.CacheDuration;
            var tenant = await tenantRepository.GetAsync(t => t.Id == tenantId, enableTracking: false, cancellationToken: cancellationToken);
            return tenant?.Slug;
        });

        if (string.IsNullOrEmpty(slug))
        {
            return platformOrigin.GetLeftPart(UriPartial.Authority);
        }

        var tenantOrigin = new UriBuilder(platformOrigin.Scheme, $"{slug}.{settings.SubdomainRoot}", platformOrigin.IsDefaultPort ? -1 : platformOrigin.Port);
        return tenantOrigin.Uri.GetLeftPart(UriPartial.Authority);
    }
}
