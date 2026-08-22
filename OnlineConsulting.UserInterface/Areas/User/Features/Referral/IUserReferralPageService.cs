namespace OnlineConsulting.UserInterface.Areas.User.Features.Referral;

public interface IUserReferralPageService
{
    Task<MyReferralViewModel> GetMyReferralPageAsync(CancellationToken cancellationToken = default);
}
