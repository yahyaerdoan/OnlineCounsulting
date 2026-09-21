namespace OnlineConsulting.UserInterface.Areas.User.Features.Referral;

public interface IUserReferralPageService
{
    /// <summary>Gets the current user's referral code, referral history, and credit summary.</summary>
    Task<MyReferralViewModel> GetMyReferralPageAsync(CancellationToken cancellationToken = default);
}
