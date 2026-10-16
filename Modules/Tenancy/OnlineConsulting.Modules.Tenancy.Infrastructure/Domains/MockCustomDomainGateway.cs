using System.Collections.Concurrent;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Domains;

/// <summary>No network calls; a domain turns ready a minute after it is added, so dev can see both states.</summary>
public class MockCustomDomainGateway : ICustomDomainGateway
{
    private static readonly TimeSpan ReadyAfter = TimeSpan.FromMinutes(1);
    private readonly ConcurrentDictionary<string, (string Host, DateTimeOffset AddedAt)> _domains = new();

    public Task<CustomDomainSetup> AddAsync(string host, CancellationToken cancellationToken = default)
    {
        var id = $"mock_domain_{Guid.NewGuid():N}";
        _domains[id] = (host, DateTimeOffset.UtcNow);
        return Task.FromResult(Setup(id, host, isReady: false));
    }

    /// <summary>Unknown ids (e.g. after a restart) count as ready.</summary>
    public Task<CustomDomainSetup> GetAsync(string providerDomainId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_domains.TryGetValue(providerDomainId, out var domain)
            ? Setup(providerDomainId, domain.Host, DateTimeOffset.UtcNow - domain.AddedAt >= ReadyAfter)
            : new CustomDomainSetup(providerDomainId, true, [], null));

    public Task RemoveAsync(string providerDomainId, CancellationToken cancellationToken = default)
    {
        _ = _domains.TryRemove(providerDomainId, out _);
        return Task.CompletedTask;
    }

    private static CustomDomainSetup Setup(string id, string host, bool isReady) =>
        new(id, isReady, [new CustomDomainDnsRecord("CNAME", DnsRecordNames.Relative(host), "sites.mock.test", isReady)], null);
}
