using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems;

/// <summary>Mirror image of AddModuleCommand - removes one à la carte module from an already-subscribed tenant, prorated refund/credit (see ISubscriptionGateway.RemoveSubscriptionItemAsync). Same Roles => [] + TenantOwnershipGuard authorization shape as AddModuleCommand.</summary>
public record RemoveModuleCommand(Guid TenantId, string ModuleKey) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class RemoveModuleHandler(
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ITenantSubscriptionItemRepository tenantSubscriptionItemRepository,
    ISubscriptionGateway subscriptionGateway,
    IFeatureFlagWriter featureFlagWriter,
    ITenantProvider tenantProvider,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<RemoveModuleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveModuleCommand request, CancellationToken cancellationToken)
    {
        if (!TenantOwnershipGuard.CallerMayManage(request.TenantId, tenantProvider.TenantId, httpContextAccessor))
            return Result.Forbidden(TenantSubscriptionItemMessages.NotAuthorizedForTenant);

        var tenantSubscription = await tenantSubscriptionRepository.GetAsync(
            s => s.TenantId == request.TenantId && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken: cancellationToken);
        if (tenantSubscription is null)
            return Result.BadRequest(TenantSubscriptionItemMessages.NoActiveSubscription);

        var item = await tenantSubscriptionItemRepository.GetAsync(
            i => i.TenantSubscriptionId == tenantSubscription.Id && i.ModuleKey == request.ModuleKey && i.Status == TenantSubscriptionItemStatuses.Active,
            cancellationToken: cancellationToken);
        if (item is null)
            return Result.NotFound(TenantSubscriptionItemMessages.ModuleNotActive);

        var providerSubscriptionItemId = item.ProviderSubscriptionItemId
            ?? throw new InvalidOperationException($"TenantSubscriptionItem {item.Id} has no ProviderSubscriptionItemId.");

        await subscriptionGateway.RemoveSubscriptionItemAsync(providerSubscriptionItemId, cancellationToken);

        await tenantSubscriptionItemRepository.DeleteAsync(item);

        await featureFlagWriter.SetAsync(request.TenantId, request.ModuleKey, false, cancellationToken);

        return Result.Success("Module removed from the tenant's subscription.");
    }
}
