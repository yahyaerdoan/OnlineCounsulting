using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Hosting;

/// <summary>The platform host maps to the default tenant, "{slug}.{SubdomainRoot}" to that tenant, and any other host to the tenant whose active custom domain it is.
/// Misses are not cached, so a tenant is reachable right after signup or activation.</summary>
public class TenantHostResolver(ITenantRepository tenantRepository, ITenantDomainRepository domainRepository, IMemoryCache cache, IOptions<TenantHostingOptions> options)
    : ITenantHostResolver
{
    private const string WwwPrefix = "www.";

    public async Task<Guid?> ResolveAsync(string host, CancellationToken cancellationToken = default)
    {
        var normalizedHost = TenantDomain.NormalizeHost(host);
        var settings = options.Value;

        if (IsPlatformOrigin(normalizedHost, settings))
        {
            return TenantDefaults.DefaultTenantId;
        }

        if (SlugOf(normalizedHost, settings) is { } slug)
        {
            return await ResolveSlugAsync(slug, settings, cancellationToken);
        }

        return await ResolveDomainAsync(normalizedHost, settings, cancellationToken);
    }

    public bool IsPlatformHost(string host)
    {
        var normalizedHost = TenantDomain.NormalizeHost(host);
        var settings = options.Value;
        return IsPlatformOrigin(normalizedHost, settings) || SlugOf(normalizedHost, settings) is not null;
    }

    // The www form of the platform host is the platform too.
    private static bool IsPlatformOrigin(string host, TenantHostingOptions settings) =>
        Uri.TryCreate(settings.PlatformOrigin, UriKind.Absolute, out var platformOrigin)
        && (host == platformOrigin.Host.ToLowerInvariant() || host == $"{WwwPrefix}{platformOrigin.Host.ToLowerInvariant()}");

    /// <summary>The part before ".{SubdomainRoot}", or null when the host is not under it.</summary>
    private static string? SlugOf(string host, TenantHostingOptions settings)
    {
        var suffix = $".{settings.SubdomainRoot.Trim().ToLowerInvariant()}";
        return settings.SubdomainRoot.Length > 0 && host.EndsWith(suffix, StringComparison.Ordinal) ? host[..^suffix.Length] : null;
    }

    private async Task<Guid?> ResolveSlugAsync(string slug, TenantHostingOptions settings, CancellationToken cancellationToken)
    {
        if (slug.Length == 0 || slug.Contains('.'))
        {
            return null;
        }

        if (cache.TryGetValue(TenantHostCache.BySlug(slug), out Guid cachedTenantId))
        {
            return cachedTenantId;
        }

        var tenant = await tenantRepository.GetAsync(t => t.Slug == slug, enableTracking: false, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return null;
        }

        _ = cache.Set(TenantHostCache.BySlug(slug), tenant.Id, settings.CacheDuration);
        return tenant.Id;
    }

    private async Task<Guid?> ResolveDomainAsync(string host, TenantHostingOptions settings, CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(TenantHostCache.ByDomain(host), out Guid cachedTenantId))
        {
            return cachedTenantId;
        }

        var domain = await domainRepository.GetAsync(d => d.Host == host && d.Status == TenantDomainStatuses.Active, enableTracking: false, cancellationToken: cancellationToken);
        if (domain is null)
        {
            return null;
        }

        _ = cache.Set(TenantHostCache.ByDomain(host), domain.TenantId, settings.CacheDuration);
        return domain.TenantId;
    }
}
