using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.GetMyTenantEmailDomain;

/// <summary>The caller's email domain; a pending one carries its live DNS records.</summary>
public record GetMyTenantEmailDomainQuery : IRequest<OperationDataResult<TenantEmailDomainResponse>>, ISecureAddRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class GetMyTenantEmailDomainHandler(ITenantEmailDomainRepository domainRepository, ITenantProvider tenantProvider, IEmailDomainGateway gateway)
    : IRequestHandler<GetMyTenantEmailDomainQuery, OperationDataResult<TenantEmailDomainResponse>>
{
    public async Task<OperationDataResult<TenantEmailDomainResponse>> Handle(GetMyTenantEmailDomainQuery request, CancellationToken cancellationToken)
    {
        var domain = await domainRepository.GetAsync(d => d.TenantId == tenantProvider.TenantId, enableTracking: false, cancellationToken: cancellationToken);
        if (domain is null)
        {
            return Result.Success(TenantEmailDomainResponse.None(), TenantEmailDomainMessages.NotSet);
        }

        if (domain.IsActive)
        {
            return Result.Success(TenantEmailDomainResponse.FromDomain(domain, null), TenantEmailDomainMessages.Retrieved);
        }

        try
        {
            var setup = await gateway.GetAsync(domain.Domain, cancellationToken);
            return Result.Success(TenantEmailDomainResponse.FromDomain(domain, setup), TenantEmailDomainMessages.Retrieved);
        }
        catch (Exception)
        {
            return Result.Success(TenantEmailDomainResponse.FromDomain(domain, null) with { Problem = TenantEmailDomainMessages.ProviderFailed }, TenantEmailDomainMessages.Retrieved);
        }
    }
}
