using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Rules;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.UpdateTenantBranding;

/// <summary>Sets the caller's own business name and logo, shown on its site, app and emails.</summary>
public record UpdateTenantBrandingCommand(string Name, Guid? LogoMediaAssetId) : IRequest<OperationResult>, ISecureAddRequest, ITenancyTransactionRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class UpdateTenantBrandingHandler(ITenantRepository tenantRepository, ITenantProvider tenantProvider, ITenantBrandCacheInvalidator cacheInvalidator)
    : IRequestHandler<UpdateTenantBrandingCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateTenantBrandingCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantProvider.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return TenantBusinessRules.TenantNotFound();
        }

        tenant.Rebrand(request.Name, request.LogoMediaAssetId);
        _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);
        cacheInvalidator.Invalidate(tenant.Id);

        return Result.Success("Branding updated.");
    }
}
