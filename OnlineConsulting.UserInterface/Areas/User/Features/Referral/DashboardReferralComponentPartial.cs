using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Referral;

public class DashboardReferralComponentPartial(IUserReferralPageService pageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() =>
        View(await pageService.GetMyReferralPageAsync());
}
