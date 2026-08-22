using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Referral;

[Area("Admin")]
[Route("admin/referrals")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class ReferralController(IReferralService referralService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await referralService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var model = await referralService.GetCompleteFormAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(CompleteReferralViewModel completeViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(completeViewModel);
        }

        var result = await referralService.CompleteAsync(completeViewModel.Id, completeViewModel.RewardAmount, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(completeViewModel);
    }
}
