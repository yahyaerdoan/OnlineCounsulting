using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Memberships.Domain;

public class PromoCode : SequentialGuidTenantEntity
{
    /// <summary>Normalized upper-case, unique per tenant.</summary>
    public required string Code { get; set; }

    /// <summary>PromoCodeDiscountTypes.Percent or .Fixed.</summary>
    public required string DiscountType { get; set; }

    /// <summary>0-100 for Percent, dollar amount for Fixed.</summary>
    public required decimal DiscountValue { get; set; }

    /// <summary>Null = unlimited redemptions.</summary>
    public int? MaxRedemptions { get; set; }
    public int RedemptionCount { get; set; }

    /// <summary>Null = never expires.</summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>False = disabled by an admin, rejected on redemption regardless of other checks.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Null = valid for any plan.</summary>
    public Guid? MembershipPlanId { get; set; }
}
