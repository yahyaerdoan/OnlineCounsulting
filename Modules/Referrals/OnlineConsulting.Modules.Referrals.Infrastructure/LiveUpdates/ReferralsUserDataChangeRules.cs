using OnlineConsulting.Modules.Referrals.Domain;
using OnlineConsulting.SharedKernel.LiveUpdates;

namespace OnlineConsulting.Modules.Referrals.Infrastructure.LiveUpdates;

/// <summary>Referral code, referral status (both sides) and account-credit ledger rows signal the users they belong to.</summary>
public static class ReferralsUserDataChangeRules
{
    public static void Configure(UserDataChangeRuleSet rules) => rules
        .ForUserProperties<ReferralCode>(UserDataTopics.Referrals, nameof(ReferralCode.UserId))
        .ForUserProperties<Referral>(UserDataTopics.Referrals, nameof(Referral.ReferrerUserId), nameof(Referral.ReferredUserId))
        .ForUserProperties<AccountCredit>(UserDataTopics.Referrals, nameof(AccountCredit.UserId));
}
