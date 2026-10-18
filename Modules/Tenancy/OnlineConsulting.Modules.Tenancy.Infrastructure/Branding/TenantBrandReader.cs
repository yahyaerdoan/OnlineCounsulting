using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Branding;

/// <summary>Cross-module implementation of ITenantBrandReader; cached since every page load and outgoing email needs it.</summary>
public class TenantBrandReader(ITenantRepository tenantRepository, ITenantEmailDomainRepository emailDomainRepository, IMemoryCache cache, IOptions<TenantBrandingOptions> options)
    : ITenantBrandReader, ITenantBrandCacheInvalidator
{
    public async Task<TenantBrand> GetAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var platformBrand = new TenantBrand(options.Value.PlatformName, null);
        if (tenantId == TenantDefaults.DefaultTenantId)
        {
            return platformBrand;
        }

        var brand = await cache.GetOrCreateAsync(CacheKey(tenantId), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = options.Value.CacheDuration;
            var tenant = await tenantRepository.GetAsync(t => t.Id == tenantId, enableTracking: false, cancellationToken: cancellationToken);
            if (tenant is null)
            {
                return null;
            }

            var emailDomain = await emailDomainRepository.GetAsync(d => d.TenantId == tenantId, enableTracking: false, cancellationToken: cancellationToken);
            return new TenantBrand(tenant.Name, tenant.LogoMediaAssetId, emailDomain is { IsActive: true } ? emailDomain.SenderAddress : null);
        });

        return brand ?? platformBrand;
    }

    public void Invalidate(Guid tenantId) => cache.Remove(CacheKey(tenantId));

    private static string CacheKey(Guid tenantId) => $"tenant-brand:{tenantId}";
}
