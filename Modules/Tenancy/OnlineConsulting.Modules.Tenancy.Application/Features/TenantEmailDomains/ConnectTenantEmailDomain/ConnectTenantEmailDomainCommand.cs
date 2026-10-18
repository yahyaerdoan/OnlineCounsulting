using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Loggings.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.Contracts;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantEmailDomains.ConnectTenantEmailDomain;

/// <summary>Registers the caller's email domain with the email provider and returns the DNS records to add.</summary>
public record ConnectTenantEmailDomainCommand(string Domain) : IRequest<OperationDataResult<TenantEmailDomainResponse>>, ISecureAddRequest, ITenancyTransactionRequest, ILogResultRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class ConnectTenantEmailDomainHandler(ITenantEmailDomainRepository domainRepository, ITenantProvider tenantProvider, IEmailDomainGateway gateway)
    : IRequestHandler<ConnectTenantEmailDomainCommand, OperationDataResult<TenantEmailDomainResponse>>
{
    public async Task<OperationDataResult<TenantEmailDomainResponse>> Handle(ConnectTenantEmailDomainCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantProvider.TenantId;
        var name = TenantDomain.NormalizeHost(request.Domain);

        if (await domainRepository.AnyAsync(d => d.TenantId == tenantId, cancellationToken: cancellationToken))
        {
            return Result.Conflict<TenantEmailDomainResponse>(TenantEmailDomainMessages.AlreadySet);
        }

        if (await domainRepository.AnyAsync(d => d.Domain == name, cancellationToken: cancellationToken))
        {
            return Result.Conflict<TenantEmailDomainResponse>(TenantEmailDomainMessages.DomainTaken);
        }

        EmailDomainSetup setup;
        try
        {
            setup = await gateway.AddAsync(name, cancellationToken);
        }
        catch (Exception)
        {
            return Result.BadGateway<TenantEmailDomainResponse>(TenantEmailDomainMessages.ProviderFailed);
        }

        var domain = TenantEmailDomain.Register(tenantId, name);
        _ = await domainRepository.AddAsync(domain, cancellationToken: cancellationToken);

        return Result.Success(TenantEmailDomainResponse.FromDomain(domain, setup), TenantEmailDomainMessages.Added);
    }
}
