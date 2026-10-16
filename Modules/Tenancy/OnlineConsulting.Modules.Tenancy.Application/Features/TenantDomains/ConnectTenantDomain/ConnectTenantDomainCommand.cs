using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Loggings.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantDomains.ConnectTenantDomain;

/// <summary>Registers the caller's own domain with the hosting provider and returns the DNS records to add.</summary>
public record ConnectTenantDomainCommand(string Host) : IRequest<OperationDataResult<TenantDomainResponse>>, ISecureAddRequest, ITenancyTransactionRequest, ILogResultRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class ConnectTenantDomainHandler(ITenantDomainRepository domainRepository, ITenantProvider tenantProvider, ITenantHostResolver hostResolver, ICustomDomainGateway gateway)
    : IRequestHandler<ConnectTenantDomainCommand, OperationDataResult<TenantDomainResponse>>
{
    public async Task<OperationDataResult<TenantDomainResponse>> Handle(ConnectTenantDomainCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantProvider.TenantId;
        var host = TenantDomain.NormalizeHost(request.Host);

        if (await domainRepository.AnyAsync(d => d.TenantId == tenantId, cancellationToken: cancellationToken))
        {
            return Result.Conflict<TenantDomainResponse>(TenantDomainMessages.AlreadySet);
        }

        if (hostResolver.IsPlatformHost(host))
        {
            return Result.Conflict<TenantDomainResponse>(TenantDomainMessages.PlatformHost);
        }

        if (await domainRepository.AnyAsync(d => d.Host == host, cancellationToken: cancellationToken))
        {
            return Result.Conflict<TenantDomainResponse>(TenantDomainMessages.HostTaken);
        }

        CustomDomainSetup setup;
        try
        {
            setup = await gateway.AddAsync(host, cancellationToken);
        }
        catch (Exception)
        {
            return Result.BadGateway<TenantDomainResponse>(TenantDomainMessages.ProviderFailed);
        }

        var domain = TenantDomain.Register(tenantId, host, setup.ProviderDomainId);
        _ = await domainRepository.AddAsync(domain, cancellationToken: cancellationToken);

        return Result.Success(TenantDomainResponse.FromDomain(domain, setup), TenantDomainMessages.Added);
    }
}
