using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetTenantById;

/// <summary>Platform-owner detail view of a single tenant - name/status plus the tenant's most recent non-cancelled subscription and every item (any status) ever billed on it, SuperAdmin only.</summary>
public record GetTenantByIdQuery(Guid TenantId) : IRequest<OperationDataResult<TenantDetailResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class GetTenantByIdHandler(
    ITenantRepository tenantRepository,
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ITenantSubscriptionItemRepository tenantSubscriptionItemRepository)
    : IRequestHandler<GetTenantByIdQuery, OperationDataResult<TenantDetailResponse>>
{
    public async Task<OperationDataResult<TenantDetailResponse>> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return Result.NotFound<TenantDetailResponse>(TenantMessages.TenantNotFound);
        }

        var subscription = await tenantSubscriptionRepository.GetAsync(
            s => s.TenantId == request.TenantId && s.Status != Domain.TenantSubscriptionStatuses.Cancelled,
            cancellationToken: cancellationToken);

        var items = subscription is null
            ? []
            : (await tenantSubscriptionItemRepository.GetListAsync(
                i => i.TenantSubscriptionId == subscription.Id,
                size: RepositoryQuerySize.Unbounded,
                cancellationToken: cancellationToken)).Items.ToList();

        return Result.Success(TenantDetailResponse.FromDomain(tenant, subscription, items), "Tenant retrieved successfully.");
    }
}
