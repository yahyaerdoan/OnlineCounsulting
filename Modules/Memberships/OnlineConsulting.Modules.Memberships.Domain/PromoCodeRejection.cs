namespace OnlineConsulting.Modules.Memberships.Domain;

/// <summary>Why a <see cref="PromoCode"/> can't be redeemed right now.</summary>
public enum PromoCodeRejection
{
    Inactive,
    Expired,
    RedemptionLimitReached,
    NotValidForPlan,
}
