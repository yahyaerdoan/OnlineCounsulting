using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetTenantById;

/// <summary>Platform-owner detail view of a single tenant - name/status plus the tenant's most recent non-cancelled subscription and every item (any status) ever billed on it, SuperAdmin only.</summary>
public record GetTenantByIdQuery(Guid TenantId) : IRequest<OperationDataResult<TenantDetailResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Cross-tenant/platform-level - a tenant admin must never reach this, even with TenantFullAccess.</summary>
    [JsonIgnore]
    public bool AllowTenantBypass => false;
}

public class GetTenantByIdHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository)
    : IRequestHandler<GetTenantByIdQuery, OperationDataResult<TenantDetailResponse>>
{
    public async Task<OperationDataResult<TenantDetailResponse>> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);

        if (tenant is null)
        {
            return Result.NotFound<TenantDetailResponse>(TenantMessages.TenantNotFound);
        }

        var subscription = await tenantSubscriptionRepository.GetWithItemsAsync(s => s.TenantId == request.TenantId && s.Status != Domain.TenantSubscriptionStatuses.Cancelled,
            enableTracking: false, cancellationToken: cancellationToken);

        return Result.Success(TenantDetailResponse.FromDomain(tenant, subscription), "Tenant retrieved successfully.");
    }
}
