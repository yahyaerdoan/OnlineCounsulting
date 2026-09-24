using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Rules;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.SuspendTenant;

/// <summary>Platform-owner action: blocks a tenant's users from every protected endpoint (see TenantStatusCheckBehavior, which exempts the SuperAdmin caller). Only Active/PastDue tenants can be suspended.</summary>
public record SuspendTenantCommand(Guid TenantId) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Cross-tenant/platform-level - a tenant admin must never reach this, even with TenantFullAccess.</summary>
    [JsonIgnore]
    public bool AllowTenantBypass => false;
}

public class SuspendTenantHandler(ITenantRepository tenantRepository) : IRequestHandler<SuspendTenantCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SuspendTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);

        if (tenant is null)
        {
            return TenantBusinessRules.TenantNotFound();
        }

        if (tenant.Status is not (TenantStatuses.Active or TenantStatuses.PastDue))
        {
            return TenantBusinessRules.NotSuspendable();
        }

        tenant.Status = TenantStatuses.Suspended;

        _ = await tenantRepository.UpdateAsync(tenant);

        return Result.Success("Tenant suspended.");
    }
}
