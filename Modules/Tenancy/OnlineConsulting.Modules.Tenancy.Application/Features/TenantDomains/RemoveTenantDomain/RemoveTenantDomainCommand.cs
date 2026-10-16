using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Loggings.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Constants;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.RemoveTenantDomain;

/// <summary>Removes the caller's domain from the provider; the site stays on its platform subdomain.</summary>
public record RemoveTenantDomainCommand : IRequest<OperationResult>, ISecureAddRequest, ITenancyTransactionRequest, ILogResultRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class RemoveTenantDomainHandler(ITenantDomainRepository domainRepository, ITenantProvider tenantProvider, ICustomDomainGateway gateway, ITenantHostCacheInvalidator cacheInvalidator)
    : IRequestHandler<RemoveTenantDomainCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveTenantDomainCommand request, CancellationToken cancellationToken)
    {
        var domain = await domainRepository.GetAsync(d => d.TenantId == tenantProvider.TenantId, cancellationToken: cancellationToken);
        if (domain is null)
        {
            return Result.NotFound(TenantDomainMessages.NotSet);
        }

        try
        {
            await gateway.RemoveAsync(domain.ProviderDomainId, cancellationToken);
        }
        catch (Exception)
        {
            return Result.BadGateway(TenantDomainMessages.ProviderFailed);
        }

        _ = await domainRepository.DeleteAsync(domain, cancellationToken: cancellationToken);
        cacheInvalidator.Invalidate(domain.TenantId, domain.Host);

        return Result.Success(TenantDomainMessages.Removed);
    }
}
