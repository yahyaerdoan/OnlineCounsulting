namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;

/// <summary>Registers tenant domains with the hosting provider, which routes them and issues their certificates.</summary>
public interface ICustomDomainGateway
{
    Task<CustomDomainSetup> AddAsync(string host, CancellationToken cancellationToken = default);

    Task<CustomDomainSetup> GetAsync(string providerDomainId, CancellationToken cancellationToken = default);

    Task RemoveAsync(string providerDomainId, CancellationToken cancellationToken = default);
}
