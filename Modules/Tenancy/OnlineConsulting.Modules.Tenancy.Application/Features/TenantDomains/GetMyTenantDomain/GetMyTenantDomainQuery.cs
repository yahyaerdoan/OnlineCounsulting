using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.GetMyTenantDomain;

/// <summary>The caller's custom domain; a pending one carries its live DNS records.</summary>
public record GetMyTenantDomainQuery : IRequest<OperationDataResult<TenantDomainResponse>>, ISecureAddRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class GetMyTenantDomainHandler(ITenantDomainRepository domainRepository, ITenantProvider tenantProvider, ICustomDomainGateway gateway)
    : IRequestHandler<GetMyTenantDomainQuery, OperationDataResult<TenantDomainResponse>>
{
    public async Task<OperationDataResult<TenantDomainResponse>> Handle(GetMyTenantDomainQuery request, CancellationToken cancellationToken)
    {
        var domain = await domainRepository.GetAsync(d => d.TenantId == tenantProvider.TenantId, enableTracking: false, cancellationToken: cancellationToken);
        if (domain is null)
        {
            return Result.Success(TenantDomainResponse.None(), TenantDomainMessages.NotSet);
        }

        if (domain.IsActive)
        {
            return Result.Success(TenantDomainResponse.FromDomain(domain, null), TenantDomainMessages.Retrieved);
        }

        try
        {
            var setup = await gateway.GetAsync(domain.ProviderDomainId, cancellationToken);
            return Result.Success(TenantDomainResponse.FromDomain(domain, setup), TenantDomainMessages.Retrieved);
        }
        catch (Exception)
        {
            return Result.Success(TenantDomainResponse.FromDomain(domain, null) with { Problem = TenantDomainMessages.ProviderFailed }, TenantDomainMessages.Retrieved);
        }
    }
}
