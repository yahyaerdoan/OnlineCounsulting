using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Memberships.Domain;

public class MembershipPlan : SequentialGuidTenantEntity
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

    /// <summary>False = retired from sale. Existing subscribers on this plan are unaffected - CustomerMembership.MembershipPlanId keeps working, only the public catalog and new subscribe attempts hide/reject it.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Null = no trial. Set only at plan creation, same immutability convention as Price/BillingCycle - UpdateMembershipPlanCommand never touches it.</summary>
    public int? TrialDays { get; set; }
}
