using Core.PersistenceLayer.Repositories.Entities;

namespace OnlineConsulting.Modules.Tenancy.Domain;

/// <summary>An à la carte module tenants can buy individually. Not tenant-scoped - shared catalog data offered to every tenant.</summary>
public class ModuleOffering : Entity<Guid>
{
    /// <summary>Matches a FeatureFlagKeys value.</summary>
    public required string Key { get; set; }

    public required string Name { get; set; }

    public required decimal Price { get; set; }

    /// <summary>SharedKernel.Payments.BillingCycles.* - shared vocabulary with ISubscriptionGateway.</summary>
    public required string BillingCycle { get; set; }

    /// <summary>Set once at creation via ISubscriptionGateway.EnsurePriceAsync - provider prices are immutable, so a price change requires a new ProviderPriceId, not an update to this one.</summary>
    public string? ProviderProductId { get; set; }
    public string? ProviderPriceId { get; set; }

    public bool IsPubliclyVisible { get; set; } = true;
}
