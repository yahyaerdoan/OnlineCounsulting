using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Hosting;

/// <summary>The platform host maps to the default tenant and "{slug}.{SubdomainRoot}" to that tenant. Misses are not cached, so a tenant is reachable right after signup.</summary>
public class TenantHostResolver(ITenantRepository tenantRepository, IMemoryCache cache, IOptions<TenantHostingOptions> options) : ITenantHostResolver
{
    public async Task<Guid?> ResolveAsync(string host, CancellationToken cancellationToken = default)
    {
        var normalizedHost = host.Trim().TrimEnd('.').ToLowerInvariant();
        var settings = options.Value;

        if (Uri.TryCreate(settings.PlatformOrigin, UriKind.Absolute, out var platformOrigin) && normalizedHost == platformOrigin.Host.ToLowerInvariant())
        {
            return TenantDefaults.DefaultTenantId;
        }

        var suffix = $".{settings.SubdomainRoot.Trim().ToLowerInvariant()}";
        if (settings.SubdomainRoot.Length == 0 || !normalizedHost.EndsWith(suffix, StringComparison.Ordinal))
        {
            return null;
        }

        var slug = normalizedHost[..^suffix.Length];
        if (slug.Length == 0 || slug.Contains('.'))
        {
            return null;
        }

        var cacheKey = $"tenant-by-slug:{slug}";
        if (cache.TryGetValue(cacheKey, out Guid cachedTenantId))
        {
            return cachedTenantId;
        }

        var tenant = await tenantRepository.GetAsync(t => t.Slug == slug, enableTracking: false, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return null;
        }

        _ = cache.Set(cacheKey, tenant.Id, settings.CacheDuration);
        return tenant.Id;
    }
}
