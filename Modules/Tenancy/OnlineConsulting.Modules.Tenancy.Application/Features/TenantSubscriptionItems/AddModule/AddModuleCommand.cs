using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
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

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.AddModule;

/// <summary>Adds one à la carte module to a subscribed tenant, billed immediately/prorated. Roles => [] deliberately - authorization is an ownership check (see TenantOwnershipGuard), not a role.</summary>
public record AddModuleCommand(Guid TenantId, string ModuleKey) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class AddModuleHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, IModuleOfferingRepository moduleOfferingRepository, ISubscriptionGateway subscriptionGateway, IFeatureFlagWriter featureFlagWriter, ITenantProvider tenantProvider, IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<AddModuleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AddModuleCommand request, CancellationToken cancellationToken)
    {
        if (!TenantOwnershipGuard.CallerMayManage(request.TenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return TenantSubscriptionItemBusinessRules.NotAuthorizedForTenant();
        }

        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);

        if (tenant is null)
        {
            return TenantSubscriptionItemBusinessRules.TenantNotFound();
        }

        var moduleOffering = await moduleOfferingRepository.GetAsync(m => m.Key == request.ModuleKey && m.IsPubliclyVisible, cancellationToken: cancellationToken);

        if (moduleOffering is null)
        {
            return TenantSubscriptionItemBusinessRules.ModuleNotFound();
        }

        var moduleOfferingPriceId = moduleOffering.ProviderPriceId
            ?? throw new InvalidOperationException($"ModuleOffering {moduleOffering.Key} has no ProviderPriceId.");

        var tenantSubscription = await tenantSubscriptionRepository.GetWithItemsAsync(s => s.TenantId == request.TenantId && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken: cancellationToken);

        if (tenantSubscription is null)
        {
            return TenantSubscriptionItemBusinessRules.NoActiveSubscription();
        }

        var providerSubscriptionId = tenantSubscription.ProviderSubscriptionId
            ?? throw new InvalidOperationException($"TenantSubscription {tenantSubscription.Id} has no ProviderSubscriptionId.");

        if (!subscriptionGateway.SupportsMultipleItems && tenantSubscription.ActiveItems.Count > 0)
        {
            return TenantSubscriptionItemBusinessRules.MultipleModulesNotSupportedByProvider();
        }

        if (tenantSubscription.HasActiveModule(request.ModuleKey))
        {
            return TenantSubscriptionItemBusinessRules.ModuleAlreadyAdded();
        }

        var item = tenantSubscription.AddModule(moduleOffering.Key, moduleOffering.Price, DateTimeOffset.UtcNow);

        _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);

        string providerSubscriptionItemId;

        if (item.ProviderSubscriptionItemId is not null)
        {
            providerSubscriptionItemId = item.ProviderSubscriptionItemId;
        }
        else
        {
            var (failure, value) = await PaymentGatewayCall.RunWithResultAsync(() => subscriptionGateway
            .AddSubscriptionItemAsync(providerSubscriptionId, moduleOfferingPriceId, idempotencyKey: $"tenant-add-module:{item.Id}", cancellationToken: cancellationToken), TenantSubscriptionItemMessages.ModuleBillingFailed);

            if (failure is not null)
            {
                tenantSubscription.FailModuleBilling(moduleOffering.Key);

                _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);

                return failure;
            }

            providerSubscriptionItemId = value ?? throw new InvalidOperationException($"Adding module {moduleOffering.Key} returned no subscription item id.");
        }

        tenantSubscription.ActivateModule(moduleOffering.Key, providerSubscriptionItemId);

        _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);

        try
        {
            await featureFlagWriter.SetAsync(request.TenantId, moduleOffering.Key, true, cancellationToken);
        }
        catch (Exception)
        {
            return Result.InternalServerError(TenantSubscriptionItemMessages.ModuleFeatureFlagFailed);
        }

        return Result.Created("Module added to the tenant's subscription.");
    }
}
