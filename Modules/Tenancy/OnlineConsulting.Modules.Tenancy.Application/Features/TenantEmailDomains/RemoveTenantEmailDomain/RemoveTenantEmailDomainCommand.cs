using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Loggings.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.RemoveTenantEmailDomain;

/// <summary>Removes the caller's email domain from the provider; emails go out from the platform address again.</summary>
public record RemoveTenantEmailDomainCommand : IRequest<OperationResult>, ISecureAddRequest, ITenancyTransactionRequest, ILogResultRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class RemoveTenantEmailDomainHandler(ITenantEmailDomainRepository domainRepository, ITenantProvider tenantProvider, IEmailDomainGateway gateway,
    ITenantBrandCacheInvalidator brandCacheInvalidator)
    : IRequestHandler<RemoveTenantEmailDomainCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveTenantEmailDomainCommand request, CancellationToken cancellationToken)
    {
        var domain = await domainRepository.GetAsync(d => d.TenantId == tenantProvider.TenantId, cancellationToken: cancellationToken);
        if (domain is null)
        {
            return Result.NotFound(TenantEmailDomainMessages.NotSet);
        }

        try
        {
            await gateway.RemoveAsync(domain.Domain, cancellationToken);
        }
        catch (Exception)
        {
            return Result.BadGateway(TenantEmailDomainMessages.ProviderFailed);
        }

        _ = await domainRepository.DeleteAsync(domain, cancellationToken: cancellationToken);
        brandCacheInvalidator.Invalidate(domain.TenantId);

        return Result.Success(TenantEmailDomainMessages.Removed);
    }
}
