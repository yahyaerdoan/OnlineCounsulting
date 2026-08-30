using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetAllTenantsPaged;

/// <summary>Paginated sibling of GetAllTenantsQuery, filterable/sortable via DynamicQuery - backs the ServerDataTable-driven Tenants admin screen. Same active-module/price enrichment join as GetAllTenantsQuery.</summary>
public record GetAllTenantsPagedQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null)
    : IRequest<OperationDataResult<Paginate<TenantSummaryResponse>>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];
}

public class GetAllTenantsPagedHandler(
    ITenantRepository tenantRepository,
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ITenantSubscriptionItemRepository tenantSubscriptionItemRepository)
    : IRequestHandler<GetAllTenantsPagedQuery, OperationDataResult<Paginate<TenantSummaryResponse>>>
{
    public async Task<OperationDataResult<Paginate<TenantSummaryResponse>>> Handle(GetAllTenantsPagedQuery request, CancellationToken cancellationToken)
    {
        var tenants = await tenantRepository.Query().ToDynamicPaginateAsync(
            request.PageRequest, request.DynamicQuery, defaultOrderBy: t => t.Name, tieBreaker: t => t.Id, cancellationToken);

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
            i => subscriptionIds.Contains(i.TenantSubscriptionId) && i.Status == TenantSubscriptionItemStatuses.Active,
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
