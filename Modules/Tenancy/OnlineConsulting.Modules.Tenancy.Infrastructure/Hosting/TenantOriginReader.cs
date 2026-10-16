using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Hosting;

/// <summary>A tenant's active custom domain, else "{scheme}://{slug}.{SubdomainRoot}{:port}" from the platform origin; the default tenant and unknown tenants get the platform origin.
/// Never derived from request headers, so a forged host cannot redirect emailed links.</summary>
public class TenantOriginReader(ITenantRepository tenantRepository, ITenantDomainRepository domainRepository, IMemoryCache cache, IOptions<TenantHostingOptions> options)
    : ITenantOriginReader
{
    public async Task<string> GetOriginAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        var platformOrigin = new Uri(settings.PlatformOrigin);

        if (tenantId == TenantDefaults.DefaultTenantId)
        {
            return platformOrigin.GetLeftPart(UriPartial.Authority);
        }

        var origin = await cache.GetOrCreateAsync(TenantHostCache.OriginHost(tenantId), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = settings.CacheDuration;

            var domain = await domainRepository.GetAsync(d => d.TenantId == tenantId && d.Status == TenantDomainStatuses.Active, enableTracking: false, cancellationToken: cancellationToken);
            if (domain is not null)
            {
                return new UriBuilder(platformOrigin.Scheme, domain.Host).Uri.GetLeftPart(UriPartial.Authority);
            }

            var tenant = await tenantRepository.GetAsync(t => t.Id == tenantId, enableTracking: false, cancellationToken: cancellationToken);
            return tenant is null
                ? null
                : new UriBuilder(platformOrigin.Scheme, $"{tenant.Slug}.{settings.SubdomainRoot}", platformOrigin.IsDefaultPort ? -1 : platformOrigin.Port).Uri.GetLeftPart(UriPartial.Authority);
        });

        return origin ?? platformOrigin.GetLeftPart(UriPartial.Authority);
    }
}
