using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Rules;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ChangeTenantTimeZone;

/// <summary>Sets the caller's own business time zone (IANA id); appointments are scheduled and every date is shown in it.</summary>
public record ChangeTenantTimeZoneCommand(string TimeZoneId) : IRequest<OperationResult>, ISecureAddRequest, ITransactionAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class ChangeTenantTimeZoneHandler(ITenantRepository tenantRepository, ITenantProvider tenantProvider, ITenantTimeZoneCacheInvalidator cacheInvalidator)
    : IRequestHandler<ChangeTenantTimeZoneCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ChangeTenantTimeZoneCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantProvider.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return TenantBusinessRules.TenantNotFound();
        }

        tenant.ChangeTimeZone(request.TimeZoneId);
        _ = await tenantRepository.UpdateAsync(tenant, cancellationToken: cancellationToken);
        cacheInvalidator.Invalidate(tenant.Id);

        return Result.Success("Business time zone updated.");
    }
}
