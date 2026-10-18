using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.CheckTenantEmailDomain;

/// <summary>Asks the email provider to verify the caller's pending domain and activates it once authenticated.</summary>
public record CheckTenantEmailDomainCommand : IRequest<OperationDataResult<TenantEmailDomainResponse>>, ISecureAddRequest, ITenancyTransactionRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class CheckTenantEmailDomainHandler(ITenantEmailDomainRepository domainRepository, ITenantProvider tenantProvider, IEmailDomainGateway gateway,
    ITenantBrandReader brandReader, ITenantBrandCacheInvalidator brandCacheInvalidator)
    : IRequestHandler<CheckTenantEmailDomainCommand, OperationDataResult<TenantEmailDomainResponse>>
{
    public async Task<OperationDataResult<TenantEmailDomainResponse>> Handle(CheckTenantEmailDomainCommand request, CancellationToken cancellationToken)
    {
        var domain = await domainRepository.GetAsync(d => d.TenantId == tenantProvider.TenantId, cancellationToken: cancellationToken);
        if (domain is null)
        {
            return Result.NotFound<TenantEmailDomainResponse>(TenantEmailDomainMessages.NotSet);
        }

        if (domain.IsActive)
        {
            return Result.Success(TenantEmailDomainResponse.FromDomain(domain, null), TenantEmailDomainMessages.Live);
        }

        var brand = await brandReader.GetAsync(domain.TenantId, cancellationToken);

        EmailDomainSetup setup;
        try
        {
            setup = await gateway.AuthenticateAsync(domain.Domain, domain.SenderAddress, brand.Name, cancellationToken);
        }
        catch (Exception)
        {
            return Result.BadGateway<TenantEmailDomainResponse>(TenantEmailDomainMessages.ProviderFailed);
        }

        if (setup.IsAuthenticated)
        {
            domain.Activate(DateTimeOffset.UtcNow);
            _ = await domainRepository.UpdateAsync(domain, cancellationToken: cancellationToken);
            brandCacheInvalidator.Invalidate(domain.TenantId);
        }

        return Result.Success(TenantEmailDomainResponse.FromDomain(domain, setup), domain.IsActive ? TenantEmailDomainMessages.Live : TenantEmailDomainMessages.StillPending);
    }
}
