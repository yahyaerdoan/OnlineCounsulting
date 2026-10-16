using Microsoft.Extensions.Caching.Memory;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Hosting;

/// <summary>Cache keys shared by host resolution and origin building, and their invalidation.</summary>
public class TenantHostCache(IMemoryCache cache) : ITenantHostCacheInvalidator
{
    public static string BySlug(string slug) => $"tenant-by-slug:{slug}";

    public static string ByDomain(string host) => $"tenant-by-domain:{host}";

    public static string OriginHost(Guid tenantId) => $"tenant-origin-host:{tenantId}";

    public void Invalidate(Guid tenantId, string host)
    {
        cache.Remove(ByDomain(host));
        cache.Remove(OriginHost(tenantId));
    }
}
