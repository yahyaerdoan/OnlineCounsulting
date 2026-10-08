using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetMyTenant;

/// <summary>Tenant-side counterpart to GetTenantByIdQuery - resolves the caller's own tenant from ITenantProvider, so any tenant admin can view it without a platform role.</summary>
public record GetMyTenantQuery : IRequest<OperationDataResult<TenantSummaryResponse>>, ISecureAddRequest
{
    public string[] Roles => [];
}

public class GetMyTenantHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, ITenantProvider tenantProvider)
    : IRequestHandler<GetMyTenantQuery, OperationDataResult<TenantSummaryResponse>>
{
    public async Task<OperationDataResult<TenantSummaryResponse>> Handle(GetMyTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetAsync(t => t.Id == tenantProvider.TenantId, cancellationToken: cancellationToken);

        if (tenant is null)
        {
            return Result.NotFound<TenantSummaryResponse>(TenantMessages.TenantNotFound);
        }

        var subscription = await tenantSubscriptionRepository.GetWithItemsAsync(s => s.TenantId == tenant.Id && s.Status != TenantSubscriptionStatuses.Cancelled, enableTracking: false,
            cancellationToken: cancellationToken);

        var items = subscription?.ActiveItems ?? [];

        var response = TenantSummaryResponse.FromDomain(tenant, [.. items.Select(i => i.ModuleKey)], items.Sum(i => i.PriceAtAddition));
        return Result.Success(response, "Tenant retrieved successfully.");
    }
}
