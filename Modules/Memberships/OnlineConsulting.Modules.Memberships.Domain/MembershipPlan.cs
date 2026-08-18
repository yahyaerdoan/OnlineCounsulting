using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Memberships.Domain;

public class MembershipPlan : TenantEntity<Guid>
{
    public required string Name { get; set; }

    /// <summary>SharedKernel.Payments.BillingCycles.* - shared vocabulary with ISubscriptionGateway, which maps it to the provider's own recurring-interval vocabulary.</summary>
    public required string BillingCycle { get; set; }

    public required decimal Price { get; set; }
    public int IncludedVisitsPerYear { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Benefits { get; set; }

    /// <summary>Set once at plan creation via ISubscriptionGateway.EnsurePriceAsync - provider prices are immutable, so a price change requires a new plan, not an update to these.</summary>
    public string? ProviderProductId { get; set; }
    public string? ProviderPriceId { get; set; }
}
