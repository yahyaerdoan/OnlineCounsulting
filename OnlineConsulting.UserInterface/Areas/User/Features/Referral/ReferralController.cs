using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Referral;

[Area("User")]
[Route("User/{controller}/{action}/{id?}")]
public class ReferralController(IUserReferralService referralService, IToastNotification toastNotification) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Redeem(RedeemCodeViewModel model, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            var result = await referralService.RedeemAsync(model.Code.Trim(), cancellationToken);
            toastNotification.ShowResult(result);
        }

        return RedirectToAction("Referral", "Dashboard", new { area = "User" });
    }
}
