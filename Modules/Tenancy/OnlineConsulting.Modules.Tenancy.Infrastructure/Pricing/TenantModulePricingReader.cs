using OnlineConsulting.Modules.Tenancy.Application.Features.TenantSubscriptions.Abstractions;
using OnlineConsulting.Modules.Tenancy.Domain;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Tenancy.Infrastructure.Pricing;

/// <summary>Cross-module implementation of ITenantModulePricingReader, backing GetFeatureFlagsQuery in the FeatureFlags module.</summary>
public class TenantModulePricingReader(
    ITenantSubscriptionRepository tenantSubscriptionRepository)
    : ITenantModulePricingReader
{
    public async Task<IReadOnlyDictionary<string, (decimal Price, bool IsPurchased)>> GetForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenantSubscription = await tenantSubscriptionRepository.GetWithItemsAsync(
            s => s.TenantId == tenantId && s.Status != TenantSubscriptionStatuses.Cancelled, enableTracking: false, cancellationToken: cancellationToken);
        if (tenantSubscription is null)
        {
            return new Dictionary<string, (decimal Price, bool IsPurchased)>();
        }

        return tenantSubscription.ActiveItems.ToDictionary(i => i.ModuleKey, i => (i.PriceAtAddition, true));
    }
}
