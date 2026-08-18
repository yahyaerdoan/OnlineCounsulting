namespace OnlineConsulting.Modules.Referrals.Application.Common;

public static class ReferralsMessages
{
    public const string ReferralNotFoundFormat = "Referral {0} was not found.";
    public const string InvalidCode = "This referral code doesn't exist.";
    public const string CannotReferSelf = "You can't redeem your own referral code.";
    public const string AlreadyReferred = "You've already redeemed a referral code.";
    public const string AlreadyRewarded = "This referral has already been rewarded.";
    public const string InsufficientCredit = "Insufficient account credit balance for this spend.";
}
