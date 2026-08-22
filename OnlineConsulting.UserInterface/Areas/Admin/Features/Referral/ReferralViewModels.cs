using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Referral;

public record ReferralListItemViewModel(
    Guid Id,
    string ReferrerName,
    string ReferredName,
    string Code,
    string Status,
    decimal? RewardAmount,
    DateTimeOffset? RewardedAt);

public class CompleteReferralViewModel
{
    public Guid Id { get; set; }
    public string ReferrerName { get; set; } = string.Empty;
    public string ReferredName { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal RewardAmount { get; set; }
}
