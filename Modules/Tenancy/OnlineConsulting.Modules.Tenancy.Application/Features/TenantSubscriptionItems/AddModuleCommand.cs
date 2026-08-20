using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
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

/// <summary>Adds one more à la carte module to an already-subscribed tenant - billed immediately, prorated (see ISubscriptionGateway.AddSubscriptionItemAsync). Roles => [] deliberately: "who may call this" is an ownership check (the caller's own tenant, or a SuperAdmin acting on another tenant), not a role - see TenantOwnershipGuard.</summary>
public record AddModuleCommand(Guid TenantId, string ModuleKey) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [];
}

public class AddModuleHandler(
    ITenantRepository tenantRepository,
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ITenantSubscriptionItemRepository tenantSubscriptionItemRepository,
    IModuleOfferingRepository moduleOfferingRepository,
    ISubscriptionGateway subscriptionGateway,
    IFeatureFlagWriter featureFlagWriter,
    ITenantProvider tenantProvider,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<AddModuleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(AddModuleCommand request, CancellationToken cancellationToken)
    {
        if (!TenantOwnershipGuard.CallerMayManage(request.TenantId, tenantProvider.TenantId, httpContextAccessor))
        {
            return Result.Forbidden(TenantSubscriptionItemMessages.NotAuthorizedForTenant);
        }

        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
        {
            return Result.NotFound(TenantSubscriptionItemMessages.TenantNotFound);
        }

        var moduleOffering = await moduleOfferingRepository.GetAsync(
            m => m.Key == request.ModuleKey && m.IsPubliclyVisible, cancellationToken: cancellationToken);
        if (moduleOffering is null)
        {
            return Result.BadRequest(TenantSubscriptionItemMessages.ModuleNotFound);
        }

        var moduleOfferingPriceId = moduleOffering.ProviderPriceId
            ?? throw new InvalidOperationException($"ModuleOffering {moduleOffering.Key} has no ProviderPriceId.");

        var tenantSubscription = await tenantSubscriptionRepository.GetAsync(
            s => s.TenantId == request.TenantId && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken: cancellationToken);
        if (tenantSubscription is null)
        {
            return Result.BadRequest(TenantSubscriptionItemMessages.NoActiveSubscription);
        }

        var providerSubscriptionId = tenantSubscription.ProviderSubscriptionId
            ?? throw new InvalidOperationException($"TenantSubscription {tenantSubscription.Id} has no ProviderSubscriptionId.");

        if (!subscriptionGateway.SupportsMultipleItems)
        {
            var hasAnyActiveItem = await tenantSubscriptionItemRepository.AnyAsync(
                i => i.TenantSubscriptionId == tenantSubscription.Id && i.Status == TenantSubscriptionItemStatuses.Active,
                cancellationToken: cancellationToken);
            if (hasAnyActiveItem)
            {
                return Result.BadRequest(TenantSubscriptionItemMessages.MultipleModulesNotSupportedByProvider);
            }
        }

        var alreadyActive = await tenantSubscriptionItemRepository.AnyAsync(
            i => i.TenantSubscriptionId == tenantSubscription.Id && i.ModuleKey == request.ModuleKey && i.Status == TenantSubscriptionItemStatuses.Active,
            cancellationToken: cancellationToken);
        if (alreadyActive)
        {
            return Result.Conflict(TenantSubscriptionItemMessages.ModuleAlreadyAdded);
        }

        var existingItem = await tenantSubscriptionItemRepository.GetAsync(
            i => i.TenantSubscriptionId == tenantSubscription.Id && i.ModuleKey == request.ModuleKey &&
                 (i.Status == TenantSubscriptionItemStatuses.Pending || i.Status == TenantSubscriptionItemStatuses.Failed),
            cancellationToken: cancellationToken);

        var item = existingItem ?? new TenantSubscriptionItem
        {
            Id = Guid.NewGuid(),
            TenantSubscriptionId = tenantSubscription.Id,
            ModuleKey = moduleOffering.Key,
            Status = TenantSubscriptionItemStatuses.Pending,
            ProviderSubscriptionItemId = null,
            PriceAtAddition = moduleOffering.Price,
            AddedAt = DateTime.UtcNow,
        };
        if (existingItem is null)
        {
            _ = await tenantSubscriptionItemRepository.AddAsync(item);
        }

        string providerSubscriptionItemId;
        if (item.ProviderSubscriptionItemId is not null)
        {
            providerSubscriptionItemId = item.ProviderSubscriptionItemId;
        }
        else
        {
            try
            {
                providerSubscriptionItemId = await subscriptionGateway.AddSubscriptionItemAsync(
                    providerSubscriptionId, moduleOfferingPriceId,
                    idempotencyKey: $"tenant-add-module:{item.Id}",
                    cancellationToken: cancellationToken);
            }
            catch (Exception)
            {
                item.Status = TenantSubscriptionItemStatuses.Failed;
                _ = await tenantSubscriptionItemRepository.UpdateAsync(item);
                return Result.BadRequest(TenantSubscriptionItemMessages.ModuleBillingFailed);
            }
        }

        item.ProviderSubscriptionItemId = providerSubscriptionItemId;
        item.Status = TenantSubscriptionItemStatuses.Active;
        _ = await tenantSubscriptionItemRepository.UpdateAsync(item);

        try
        {
            await featureFlagWriter.SetAsync(request.TenantId, moduleOffering.Key, true, cancellationToken);
        }
        catch (Exception)
        {
            return Result.BadRequest(TenantSubscriptionItemMessages.ModuleFeatureFlagFailed);
        }

        return Result.Created("Module added to the tenant's subscription.");
    }
}
