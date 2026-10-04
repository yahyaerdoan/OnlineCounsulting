using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Rules;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.RemoveModule;

/// <summary>Mirror of AddModuleCommand - removes one à la carte module, prorated refund/credit. Same Roles => [] + TenantOwnershipGuard authorization shape.</summary>
public record RemoveModuleCommand(Guid TenantId, string ModuleKey) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class RemoveModuleHandler(ITenantSubscriptionRepository tenantSubscriptionRepository, ISubscriptionGateway subscriptionGateway, IFeatureFlagWriter featureFlagWriter, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<RemoveModuleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveModuleCommand request, CancellationToken cancellationToken)
    {
        if (!TenantOwnershipGuard.CallerMayManage(request.TenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return TenantSubscriptionItemBusinessRules.NotAuthorizedForTenant();
        }

        var tenantSubscription = await tenantSubscriptionRepository.GetWithItemsAsync(s => s.TenantId == request.TenantId && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken: cancellationToken);

        if (tenantSubscription is null)
        {
            return TenantSubscriptionItemBusinessRules.NoActiveSubscription();
        }

        var item = tenantSubscription.ActiveItems.FirstOrDefault(i => i.ModuleKey == request.ModuleKey);

        if (item is null)
        {
            return TenantSubscriptionItemBusinessRules.ModuleNotActive();
        }

        if (!tenantSubscription.CanRemoveModule(request.ModuleKey))
        {
            return TenantSubscriptionItemBusinessRules.CannotRemoveLastModule();
        }

        var providerSubscriptionItemId = item.ProviderSubscriptionItemId
            ?? throw new InvalidOperationException($"TenantSubscriptionItem {item.Id} has no ProviderSubscriptionItemId.");

        var failure = await PaymentGatewayCall.RunAsync(() => subscriptionGateway.RemoveSubscriptionItemAsync(providerSubscriptionItemId, cancellationToken), TenantSubscriptionItemMessages.ModuleRemovalFailed);

        if (failure is not null)
        {
            return failure;
        }

        tenantSubscription.RemoveModule(request.ModuleKey, DateTimeOffset.UtcNow);

        _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);

        await featureFlagWriter.SetAsync(request.TenantId, request.ModuleKey, false, cancellationToken);

        return Result.Success("Module removed from the tenant's subscription.");
    }
}
