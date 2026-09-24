using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Rules;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ReactivateTenant;

/// <summary>Platform-owner action: lifts a suspension back to Active; billing untouched, does not retry payment (see ActivateTenantSubscriptionCommand).</summary>
public record ReactivateTenantCommand(Guid TenantId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Cross-tenant/platform-level - a tenant admin must never reach this, even with TenantFullAccess.</summary>
    [JsonIgnore]
    public bool AllowTenantBypass => false;
}

public class ReactivateTenantHandler(ITenantRepository tenantRepository) : IRequestHandler<ReactivateTenantCommand, OperationResult>
{
    public async Task<OperationResult> Handle(ReactivateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);

        if (tenant is null)
        {
            return TenantBusinessRules.TenantNotFound();
        }

        if (tenant.Status != TenantStatuses.Suspended)
        {
            return TenantBusinessRules.NotReactivatable();
        }

        tenant.Status = TenantStatuses.Active;

        _ = await tenantRepository.UpdateAsync(tenant);

        return Result.Success("Tenant reactivated.");
    }
}
