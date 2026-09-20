using OnlineConsulting.Modules.Memberships.Application.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Constants;
using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Common;

/// <summary>Shared validity checks + discount math, used by both ValidatePromoCodeCommand (preview,
/// no redemption side effects) and SubscribeToMembershipCommand (the real charge).</summary>
public static class PromoCodeEvaluator
{
    public static (bool IsValid, string? Error, decimal DiscountAmount) Evaluate(PromoCode? promo, MembershipPlan plan, bool alreadyRedeemedByUser)
    {
        if (promo is null)
        {
            return (false, PromoCodeMessages.NotFound, 0);
        }

        if (!promo.IsActive)
        {
            return (false, PromoCodeMessages.Inactive, 0);
        }

        if (promo.ExpiresAt is { } expiresAt && expiresAt < DateTimeOffset.UtcNow)
        {
            return (false, PromoCodeMessages.Expired, 0);
        }

        if (promo.MaxRedemptions is { } max && promo.RedemptionCount >= max)
        {
            return (false, PromoCodeMessages.RedemptionLimitReached, 0);
        }

        if (promo.MembershipPlanId is { } planId && planId != plan.Id)
        {
            return (false, PromoCodeMessages.NotValidForPlan, 0);
        }

        if (alreadyRedeemedByUser)
        {
            return (false, PromoCodeMessages.AlreadyRedeemed, 0);
        }

        var amount = promo.DiscountType == PromoCodeDiscountTypes.Percent
            ? Math.Round(plan.Price * promo.DiscountValue / 100m, 2)
            : promo.DiscountValue;

        return (true, null, Math.Min(amount, plan.Price));
    }
}
