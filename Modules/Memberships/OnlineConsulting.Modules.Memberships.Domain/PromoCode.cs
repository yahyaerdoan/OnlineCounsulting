using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Memberships.Domain;

/// <summary>A discount code for membership signups. Redemptions only go through <see cref="Redeem"/>, which enforces its limits.</summary>
public class PromoCode : SequentialGuidTenantEntity
{
    private PromoCode()
    {
    }

    /// <summary>Upper case, unique per tenant.</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>One of <see cref="PromoCodeDiscountTypes"/>.</summary>
    public string DiscountType { get; private set; } = PromoCodeDiscountTypes.Percent;

    /// <summary>0-100 for Percent, an amount for Fixed.</summary>
    public decimal DiscountValue { get; private set; }

    /// <summary>Null means unlimited.</summary>
    public int? MaxRedemptions { get; private set; }

    public int RedemptionCount { get; private set; }

    /// <summary>Null means it never expires.</summary>
    public DateTimeOffset? ExpiresAt { get; private set; }

    public bool IsActive { get; private set; } = true;

    /// <summary>Null means valid for any plan.</summary>
    public Guid? MembershipPlanId { get; private set; }

    /// <summary>Creates an active code; the code is trimmed and upper-cased.</summary>
    public static PromoCode Create(string code, string discountType, decimal discountValue, int? maxRedemptions, DateTimeOffset? expiresAt, Guid? membershipPlanId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        if (discountType is not (PromoCodeDiscountTypes.Percent or PromoCodeDiscountTypes.Fixed))
        {
            throw new ArgumentException($"Unknown discount type '{discountType}'.", nameof(discountType));
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(discountValue);
        if (discountType == PromoCodeDiscountTypes.Percent)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(discountValue, 100);
        }

        if (maxRedemptions is { } max)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(max, nameof(maxRedemptions));
        }

        return new PromoCode
        {
            Code = Normalize(code),
            DiscountType = discountType,
            DiscountValue = discountValue,
            MaxRedemptions = maxRedemptions,
            ExpiresAt = expiresAt,
            MembershipPlanId = membershipPlanId,
        };
    }

    /// <summary>The form codes are stored and looked up in.</summary>
    public static string Normalize(string code) => code.Trim().ToUpperInvariant();

    /// <summary>Why the code can't be used for <paramref name="membershipPlanId"/> at <paramref name="now"/>; null when it can.</summary>
    public PromoCodeRejection? CheckRedeemable(Guid membershipPlanId, DateTimeOffset now) => this switch
    {
        { IsActive: false } => PromoCodeRejection.Inactive,
        { ExpiresAt: { } expiresAt } when expiresAt < now => PromoCodeRejection.Expired,
        { MaxRedemptions: { } max } when RedemptionCount >= max => PromoCodeRejection.RedemptionLimitReached,
        { MembershipPlanId: { } planId } when planId != membershipPlanId => PromoCodeRejection.NotValidForPlan,
        _ => null,
    };

    /// <summary>The discount on a plan of <paramref name="planPrice"/>, never more than the price.</summary>
    public decimal DiscountFor(decimal planPrice)
    {
        var amount = DiscountType == PromoCodeDiscountTypes.Percent ? Math.Round(planPrice * DiscountValue / 100m, 2) : DiscountValue;
        return Math.Min(amount, planPrice);
    }

    /// <summary>Counts one use. Requires <see cref="CheckRedeemable"/> to pass.</summary>
    public void Redeem(Guid membershipPlanId, DateTimeOffset now)
    {
        if (CheckRedeemable(membershipPlanId, now) is { } rejection)
        {
            throw new InvalidOperationException($"Promo code {Code} cannot be redeemed: {rejection}.");
        }

        RedemptionCount++;
    }

    /// <summary>Enables (true) or disables (false) the code.</summary>
    public void SetActive(bool isActive) => IsActive = isActive;
}
