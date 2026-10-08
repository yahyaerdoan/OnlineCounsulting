using OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.Constants;
using OnlineConsulting.Modules.Memberships.Domain;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes;

/// <summary>Turns a promo code check into a user-facing result, for both the preview (ValidatePromoCode) and the real signup.</summary>
public static class PromoCodeEvaluator
{
    public static (bool IsValid, string? Error, decimal DiscountAmount) Evaluate(PromoCode? promo, MembershipPlan plan, bool alreadyRedeemedByUser, DateTimeOffset now)
    {
        if (promo is null)
        {
            return (false, PromoCodeMessages.NotFound, 0);
        }

        var error = promo.CheckRedeemable(plan.Id, now) switch
        {
            PromoCodeRejection.Inactive => PromoCodeMessages.Inactive,
            PromoCodeRejection.Expired => PromoCodeMessages.Expired,
            PromoCodeRejection.RedemptionLimitReached => PromoCodeMessages.RedemptionLimitReached,
            PromoCodeRejection.NotValidForPlan => PromoCodeMessages.NotValidForPlan,
            _ => alreadyRedeemedByUser ? PromoCodeMessages.AlreadyRedeemed : null,
        };

        return error is null ? (true, null, promo.DiscountFor(plan.Price)) : (false, error, 0);
    }
}
