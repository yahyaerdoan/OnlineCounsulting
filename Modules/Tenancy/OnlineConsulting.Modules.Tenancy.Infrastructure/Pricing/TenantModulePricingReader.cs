using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptionItems.Abstractions;
using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Pricing;

/// <summary>Cross-module implementation of ITenantModulePricingReader, backing GetFeatureFlagsQuery in the FeatureFlags module.</summary>
public class TenantModulePricingReader(
    ITenantSubscriptionRepository tenantSubscriptionRepository,
    ITenantSubscriptionItemRepository tenantSubscriptionItemRepository)
    : ITenantModulePricingReader
{
    public async Task<IReadOnlyDictionary<string, (decimal Price, bool IsPurchased)>> GetForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenantSubscription = await tenantSubscriptionRepository.GetAsync(
            s => s.TenantId == tenantId && s.Status != TenantSubscriptionStatuses.Cancelled, cancellationToken: cancellationToken);
        if (tenantSubscription is null)
        {
            return new Dictionary<string, (decimal Price, bool IsPurchased)>();
        }

        var items = await tenantSubscriptionItemRepository.GetAllAsync(i => i.TenantSubscriptionId == tenantSubscription.Id && i.Status == TenantSubscriptionItemStatuses.Active,
            cancellationToken: cancellationToken);

        return items.ToDictionary(i => i.ModuleKey, i => (i.PriceAtAddition, true));
    }
}
