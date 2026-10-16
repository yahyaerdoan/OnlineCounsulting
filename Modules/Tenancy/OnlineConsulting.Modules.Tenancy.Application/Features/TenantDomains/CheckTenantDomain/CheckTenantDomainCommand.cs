using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.CheckTenantDomain;

/// <summary>Checks the caller's pending domain now instead of waiting for the background poller.</summary>
public record CheckTenantDomainCommand : IRequest<OperationDataResult<TenantDomainResponse>>, ISecureAddRequest, ITenancyTransactionRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class CheckTenantDomainHandler(ITenantDomainRepository domainRepository, ITenantProvider tenantProvider, TenantDomainActivator activator)
    : IRequestHandler<CheckTenantDomainCommand, OperationDataResult<TenantDomainResponse>>
{
    public async Task<OperationDataResult<TenantDomainResponse>> Handle(CheckTenantDomainCommand request, CancellationToken cancellationToken)
    {
        var domain = await domainRepository.GetAsync(d => d.TenantId == tenantProvider.TenantId, cancellationToken: cancellationToken);
        if (domain is null)
        {
            return Result.NotFound<TenantDomainResponse>(TenantDomainMessages.NotSet);
        }

        CustomDomainSetup setup;
        try
        {
            setup = await activator.SyncAsync(domain, cancellationToken);
        }
        catch (Exception)
        {
            return Result.BadGateway<TenantDomainResponse>(TenantDomainMessages.ProviderFailed);
        }

        return Result.Success(TenantDomainResponse.FromDomain(domain, setup), domain.IsActive ? TenantDomainMessages.Live : TenantDomainMessages.StillPending);
    }
}
