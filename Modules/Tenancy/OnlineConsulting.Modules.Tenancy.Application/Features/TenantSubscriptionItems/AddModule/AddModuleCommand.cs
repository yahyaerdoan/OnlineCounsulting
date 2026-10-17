using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using Core.SecurityLayer.Constants;
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Rules;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.CurrentUser;
using OnlineConsulting.SharedKernel.FeatureFlags;
using OnlineConsulting.SharedKernel.Payments;
using OnlineConsulting.SharedKernel.Tenancy;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.AddModule;

/// <summary>Adds one à la carte module to a subscribed tenant, charging the prorated rest of the period now and emailing the receipt;
/// a module already ending just renews again, with no charge. Changes the tenant's bill, so only its admins (or a SuperAdmin) may,
/// and TenantOwnershipGuard keeps a tenant admin to their own tenant.</summary>
public record AddModuleCommand(Guid TenantId, string ModuleKey) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [GeneralOperationClaims.Admin, GlobalOperationClaims.SuperAdmin];
}

public class AddModuleHandler(ITenantRepository tenantRepository, ITenantSubscriptionRepository tenantSubscriptionRepository, IModuleOfferingRepository moduleOfferingRepository, ISubscriptionGateway subscriptionGateway, IFeatureFlagWriter featureFlagWriter, ITenantProvider tenantProvider, ICurrentUserAccessor currentUserAccessor, TenantReceiptSender receiptSender,
    ILogger<AddModuleHandler> logger)
    : IRequestHandler<AddModuleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AddModuleCommand request, CancellationToken cancellationToken)
    {
        if (!TenantOwnershipGuard.CallerMayManage(request.TenantId, tenantProvider.TenantId, currentUserAccessor))
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

        if (tenantSubscription.IsModuleEnding(request.ModuleKey))
        {
            return await ResumeAsync(tenantSubscription, providerSubscriptionId, moduleOfferingPriceId, request.ModuleKey, cancellationToken);
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
            .AddSubscriptionItemAsync(providerSubscriptionId, moduleOfferingPriceId, ProrationBilling.ChargeNow, idempotencyKey: $"tenant-add-module:{item.Id}", cancellationToken: cancellationToken),
                TenantSubscriptionItemMessages.ModuleBillingFailed, logger);

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

        await receiptSender.SendLatestAsync(tenant, providerSubscriptionId, cancellationToken);

        return Result.Created(TenantSubscriptionItemMessages.ModuleAdded);
    }

    /// <summary>Already paid for this period, so the provider item comes back without proration.</summary>
    private async Task<OperationResult> ResumeAsync(TenantSubscription tenantSubscription, string providerSubscriptionId, string priceId, string moduleKey, CancellationToken cancellationToken)
    {
        var (failure, providerSubscriptionItemId) = await PaymentGatewayCall.RunWithResultAsync(() => subscriptionGateway
            .AddSubscriptionItemAsync(providerSubscriptionId, priceId, ProrationBilling.None, cancellationToken: cancellationToken), TenantSubscriptionItemMessages.ModuleBillingFailed, logger);

        if (failure is not null)
        {
            return failure;
        }

        tenantSubscription.ResumeModule(moduleKey, providerSubscriptionItemId);
        _ = await tenantSubscriptionRepository.UpdateAsync(tenantSubscription, cancellationToken: cancellationToken);

        return Result.Success(TenantSubscriptionItemMessages.ModuleRemovalUndone);
    }
}
