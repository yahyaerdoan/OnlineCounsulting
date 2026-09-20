using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Persistence;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetMyTenant;

/// <summary>Tenant-side self-service counterpart to GetTenantByIdQuery (SuperAdmin only, takes an
/// arbitrary id) - always resolves the CALLER's own tenant from ITenantProvider, so any tenant admin
/// can see their own subscription/modules without needing a platform-level role.</summary>
public record GetMyTenantQuery : IRequest<OperationDataResult<TenantSummaryResponse>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class GetMyTenantHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, ITenantSubscriptionItemRepository tenantSubscriptionItemRepository, ITenantProvider tenantProvider)
    : IRequestHandler<GetMyTenantQuery, OperationDataResult<TenantSummaryResponse>>
{
    public async Task<OperationDataResult<TenantSummaryResponse>> Handle(GetMyTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantProvider.TenantId, cancellationToken: cancellationToken);

        if (tenant is null)
        {
            return Result.NotFound<TenantSummaryResponse>(TenantMessages.TenantNotFound);
        }

        var subscription = await tenantSubscriptionRepository.GetAsync(s => s.TenantId == tenant.Id && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken: cancellationToken);

        var items = subscription is null
            ? []
            : (await tenantSubscriptionItemRepository
            .GetListAsync(i => i.TenantSubscriptionId == subscription.Id && i.Status == TenantSubscriptionItemStatuses.Active, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken)).Items.ToList();

        var response = TenantSummaryResponse.FromDomain(tenant, [.. items.Select(i => i.ModuleKey)], items.Sum(i => i.PriceAtAddition));
        return Result.Success(response, "Tenant retrieved successfully.");
    }
}
