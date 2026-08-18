using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetAllTenants;

/// <summary>Platform-owner dashboard listing of every tenant, SuperAdmin only. Active modules and total active price are derived from each tenant's non-cancelled TenantSubscription and its non-removed TenantSubscriptionItem rows - a small per-page join rather than a new repository method, since ITenantSubscriptionRepository/ITenantSubscriptionItemRepository can already filter by a set of tenant/subscription ids.</summary>
public record GetAllTenantsQuery(PageRequest PageRequest) : IRequest<OperationDataResult<Paginate<TenantSummaryResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class GetAllTenantsHandler(
    ITenantRepository tenantRepository,
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ITenantSubscriptionItemRepository tenantSubscriptionItemRepository)
    : IRequestHandler<GetAllTenantsQuery, OperationDataResult<Paginate<TenantSummaryResponse>>>
{
    public async Task<OperationDataResult<Paginate<TenantSummaryResponse>>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = await tenantRepository.GetListAsync(
            index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize, cancellationToken: cancellationToken);

        if (tenants.Items.Count == 0)
        {
            return Result.Success(new Paginate<TenantSummaryResponse>
            {
                Items = [],
                Index = tenants.Index,
                Size = tenants.Size,
                Count = tenants.Count,
                Pages = tenants.Pages,
            }, "No tenants found.");
        }

        var tenantIds = tenants.Items.Select(t => t.Id).ToList();

        var subscriptions = await tenantSubscriptionRepository.GetListAsync(
            s => tenantIds.Contains(s.TenantId) && s.Status != TenantSubscriptionStatuses.Cancelled,
            size: RepositoryQuerySize.Unbounded,
            cancellationToken: cancellationToken);

        var subscriptionIdsByTenantId = subscriptions.Items.ToDictionary(s => s.Id, s => s.TenantId);
        var subscriptionIds = subscriptionIdsByTenantId.Keys.ToList();

        var items = await tenantSubscriptionItemRepository.GetListAsync(
            i => subscriptionIds.Contains(i.TenantSubscriptionId) && i.DeletedDate == null,
            size: RepositoryQuerySize.Unbounded,
            cancellationToken: cancellationToken);

        var itemsByTenantId = items.Items
            .GroupBy(i => subscriptionIdsByTenantId[i.TenantSubscriptionId])
            .ToDictionary(g => g.Key, g => g.ToList());

        var response = new Paginate<TenantSummaryResponse>
        {
            Items = [.. tenants.Items.Select(t =>
            {
                var tenantItems = itemsByTenantId.GetValueOrDefault(t.Id, []);
                return TenantSummaryResponse.FromDomain(
                    t,
                    [.. tenantItems.Select(i => i.ModuleKey)],
                    tenantItems.Sum(i => i.PriceAtAddition));
            })],
            Index = tenants.Index,
            Size = tenants.Size,
            Count = tenants.Count,
            Pages = tenants.Pages,
        };

        return Result.Success(response, "Tenants retrieved successfully.");
    }
}
