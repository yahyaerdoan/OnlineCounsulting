using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Referral;

public record ReferralHistoryItemViewModel(Guid Id, string Code, string Status, decimal? RewardAmount, DateTimeOffset? RewardedAt);

public record CreditEntryViewModel(Guid Id, decimal Amount, string Reason, string SourceType);

public record MyReferralViewModel(
    string Code,
    List<ReferralHistoryItemViewModel> MyReferrals,
    decimal CreditBalance,
    List<CreditEntryViewModel> CreditEntries);

public class RedeemCodeViewModel
{
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;
}
