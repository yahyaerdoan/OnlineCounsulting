using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.ApplicationLayer.Requests.Lists;
using Core.ApplicationLayer.Requests.Page;
using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.ListTenants;

/// <summary>Paginated sibling of GetAllTenantsQuery, filterable/sortable via DynamicQuery - backs the ServerDataTable-driven Tenants admin screen. Same active-module/price enrichment join as GetAllTenantsQuery.</summary>
public record ListTenantsQuery(PageRequest PageRequest, DynamicQuery? DynamicQuery = null, string? Status = null)
    : IRequest<OperationDataResult<Paginate<TenantSummaryResponse>>>, ISecureAddRequest, IDynamicListRequest
{
    public static IReadOnlySet<string> QueryableFields { get; } = new HashSet<string>([nameof(Tenant.Name), nameof(Tenant.Slug), nameof(Tenant.PrimaryContactEmail), nameof(Tenant.CreatedDate)]);

    [JsonIgnore]
    public string[] Roles => [GlobalOperationClaims.SuperAdmin];

    /// <summary>Cross-tenant/platform-level - a tenant admin must never reach this, even with TenantFullAccess.</summary>
    [JsonIgnore]
    public bool AllowTenantBypass => false;
}

public class ListTenantsHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository)
    : IRequestHandler<ListTenantsQuery, OperationDataResult<Paginate<TenantSummaryResponse>>>
{
    public async Task<OperationDataResult<Paginate<TenantSummaryResponse>>> Handle(ListTenantsQuery request, CancellationToken cancellationToken)
    {
        var query = string.IsNullOrWhiteSpace(request.Status) ? tenantRepository.Query() : tenantRepository.Query().Where(t => t.Status == request.Status);

        var tenants = await query.ToDynamicPaginateAsync(request, defaultOrderBy: t => t.Name, tieBreaker: t => t.Id, cancellationToken: cancellationToken);

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

        var subscriptions = await tenantSubscriptionRepository
            .GetAllWithItemsAsync(s => tenantIds.Contains(s.TenantId) && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken);

        var itemsByTenantId = subscriptions.GroupBy(s => s.TenantId).ToDictionary(g => g.Key, g => g.SelectMany(s => s.ActiveItems).ToList());

        var response = new Paginate<TenantSummaryResponse>
        {
            Items = [.. tenants.Items.Select(t =>
            {
                var tenantItems = itemsByTenantId.GetValueOrDefault(t.Id, []);
                return TenantSummaryResponse.FromDomain(t,[.. tenantItems.Select(i => i.ModuleKey)],tenantItems.Sum(i => i.PriceAtAddition));
            })],
            Index = tenants.Index,
            Size = tenants.Size,
            Count = tenants.Count,
            Pages = tenants.Pages,
        };

        return Result.Success(response, "Tenants retrieved successfully.");
    }
}
