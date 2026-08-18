using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>One purchased module line within a tenant's subscription. Not tenant-scoped itself - lives alongside Tenant/TenantSubscription in the platform-owner's own schema.</summary>
public class TenantSubscriptionItem : Entity<Guid>
{
    /// <summary>Plain id, no navigation.</summary>
    public required Guid TenantSubscriptionId { get; set; }

    /// <summary>Matches a FeatureFlagKeys value / ModuleOffering.Key.</summary>
    public required string ModuleKey { get; set; }

    /// <summary>Stripe SubscriptionItem id - add/remove operations go through this.</summary>
    public string? ProviderSubscriptionItemId { get; set; }

    /// <summary>Price at the moment this module was added - preserves history even if ModuleOffering.Price changes later.</summary>
    public required decimal PriceAtAddition { get; set; }

    public required DateTime AddedAt { get; set; }
}
