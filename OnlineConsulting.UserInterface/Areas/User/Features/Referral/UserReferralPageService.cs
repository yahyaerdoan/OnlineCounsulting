namespace OnlineConsulting.UserInterface.Areas.User.Features.Referral;

public class UserReferralPageService(IUserReferralService referralService) : IUserReferralPageService
{
    public async Task<MyReferralViewModel> GetMyReferralPageAsync(CancellationToken cancellationToken = default)
    {
        var codeTask = referralService.GetMyCodeAsync(cancellationToken);
        var referralsTask = referralService.GetMyReferralsAsync(cancellationToken);
        var creditTask = referralService.GetMyCreditAsync(cancellationToken);
        await Task.WhenAll(codeTask, referralsTask, creditTask);

        var myReferrals = referralsTask.Result
            .Select(r => new ReferralHistoryItemViewModel(r.Id, r.Code, r.Status, r.RewardAmount, r.RewardedAt))
            .ToList();

        var creditEntries = creditTask.Result.Entries
            .Select(e => new CreditEntryViewModel(e.Id, e.Amount, e.Reason, e.SourceType))
            .ToList();

        return new MyReferralViewModel(codeTask.Result, myReferrals, creditTask.Result.Balance, creditEntries);
    }
}
