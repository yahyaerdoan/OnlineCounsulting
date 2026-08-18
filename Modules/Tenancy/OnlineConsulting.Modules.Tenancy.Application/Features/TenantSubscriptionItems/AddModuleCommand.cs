using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Constants;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Abstractions;
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
            return Result.Forbidden(TenantSubscriptionItemMessages.NotAuthorizedForTenant);

        var tenant = await tenantRepository.GetAsync(t => t.Id == request.TenantId, cancellationToken: cancellationToken);
        if (tenant is null)
            return Result.NotFound(TenantSubscriptionItemMessages.TenantNotFound);

        var moduleOffering = await moduleOfferingRepository.GetAsync(
            m => m.Key == request.ModuleKey && m.IsPubliclyVisible, cancellationToken: cancellationToken);
        if (moduleOffering is null)
            return Result.BadRequest(TenantSubscriptionItemMessages.ModuleNotFound);

        var moduleOfferingPriceId = moduleOffering.ProviderPriceId
            ?? throw new InvalidOperationException($"ModuleOffering {moduleOffering.Key} has no ProviderPriceId.");

        var tenantSubscription = await tenantSubscriptionRepository.GetAsync(
            s => s.TenantId == request.TenantId && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken: cancellationToken);
        if (tenantSubscription is null)
            return Result.BadRequest(TenantSubscriptionItemMessages.NoActiveSubscription);

        var providerSubscriptionId = tenantSubscription.ProviderSubscriptionId
            ?? throw new InvalidOperationException($"TenantSubscription {tenantSubscription.Id} has no ProviderSubscriptionId.");

        var alreadyActive = await tenantSubscriptionItemRepository.AnyAsync(
            i => i.TenantSubscriptionId == tenantSubscription.Id && i.ModuleKey == request.ModuleKey && i.DeletedDate == null,
            cancellationToken: cancellationToken);
        if (alreadyActive)
            return Result.Conflict(TenantSubscriptionItemMessages.ModuleAlreadyAdded);

        var providerSubscriptionItemId = await subscriptionGateway.AddSubscriptionItemAsync(providerSubscriptionId, moduleOfferingPriceId, cancellationToken);

        var item = new TenantSubscriptionItem
        {
            Id = Guid.NewGuid(),
            TenantSubscriptionId = tenantSubscription.Id,
            ModuleKey = moduleOffering.Key,
            ProviderSubscriptionItemId = providerSubscriptionItemId,
            PriceAtAddition = moduleOffering.Price,
            AddedAt = DateTime.UtcNow,
        };

        await tenantSubscriptionItemRepository.AddAsync(item);

        await featureFlagWriter.SetAsync(request.TenantId, moduleOffering.Key, true, cancellationToken);

        return Result.Created("Module added to the tenant's subscription.");
    }
}
