using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Domain;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains;

/// <summary>Pulls a domain's provider state and activates it once ready; shared by the check endpoint and the background poller.</summary>
public class TenantDomainActivator(ITenantDomainRepository domainRepository, ICustomDomainGateway gateway, ITenantHostCacheInvalidator cacheInvalidator)
{
    public async Task<CustomDomainSetup> SyncAsync(TenantDomain domain, CancellationToken cancellationToken)
    {
        var setup = await gateway.GetAsync(domain.ProviderDomainId, cancellationToken);
        if (!setup.IsReady || domain.IsActive)
        {
            return setup;
        }

        domain.Activate(DateTimeOffset.UtcNow);
        _ = await domainRepository.UpdateAsync(domain, cancellationToken: cancellationToken);
        cacheInvalidator.Invalidate(domain.TenantId, domain.Host);

        return setup;
    }
}
