using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.TimeZones;

/// <summary>Cross-module implementation of ITenantTimeZoneReader; cached since every availability lookup and appointment message needs it.</summary>
public class TenantTimeZoneReader(ITenantRepository tenantRepository, IMemoryCache cache, IOptions<TenantTimeZoneOptions> options)
    : ITenantTimeZoneReader, ITenantTimeZoneCacheInvalidator
{
    public async Task<TimeZoneInfo> GetAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var timeZoneId = await cache.GetOrCreateAsync(CacheKey(tenantId), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = options.Value.CacheDuration;
            var tenant = await tenantRepository.GetAsync(t => t.Id == tenantId, enableTracking: false, cancellationToken: cancellationToken);
            return tenant?.TimeZoneId ?? options.Value.DefaultTimeZoneId;
        });

        return BusinessTimeZones.FindOrDefault(timeZoneId);
    }

    public void Invalidate(Guid tenantId) => cache.Remove(CacheKey(tenantId));

    private static string CacheKey(Guid tenantId) => $"tenant-time-zone:{tenantId}";
}
