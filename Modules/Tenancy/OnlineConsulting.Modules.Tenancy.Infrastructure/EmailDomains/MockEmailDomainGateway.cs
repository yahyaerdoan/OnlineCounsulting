using System.Collections.Concurrent;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.EmailDomains;

/// <summary>No network calls; a domain authenticates a minute after it is added, so dev can see both states.</summary>
public class MockEmailDomainGateway : IEmailDomainGateway
{
    private static readonly TimeSpan ReadyAfter = TimeSpan.FromMinutes(1);
    private readonly ConcurrentDictionary<string, DateTimeOffset> _addedAt = new();

    public Task<EmailDomainSetup> AddAsync(string domain, CancellationToken cancellationToken = default)
    {
        _addedAt[domain] = DateTimeOffset.UtcNow;
        return GetAsync(domain, cancellationToken);
    }

    /// <summary>Unknown domains (e.g. after a restart) count as authenticated.</summary>
    public Task<EmailDomainSetup> GetAsync(string domain, CancellationToken cancellationToken = default)
    {
        var isReady = !_addedAt.TryGetValue(domain, out var addedAt) || DateTimeOffset.UtcNow - addedAt >= ReadyAfter;
        return Task.FromResult(Setup(isReady));
    }

    public Task<EmailDomainSetup> AuthenticateAsync(string domain, string senderAddress, string senderName, CancellationToken cancellationToken = default) =>
        GetAsync(domain, cancellationToken);

    public Task RemoveAsync(string domain, CancellationToken cancellationToken = default)
    {
        _ = _addedAt.TryRemove(domain, out _);
        return Task.CompletedTask;
    }

    private static EmailDomainSetup Setup(bool isReady) => new(isReady,
    [
        new CustomDomainDnsRecord("TXT", "@", "brevo-code:mock", isReady),
        new CustomDomainDnsRecord("TXT", "mail._domainkey", "k=rsa;p=MOCK", isReady),
        new CustomDomainDnsRecord("TXT", "_dmarc", "v=DMARC1; p=none", isReady),
    ], null);
}
