using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.FeatureFlag;

[Area("Admin")]
[Route("admin/feature-flags")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class FeatureFlagController(IFeatureFlagService featureFlagService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await featureFlagService.GetAllAsync(cancellationToken));

    [HttpPost("toggle")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(string key, bool isEnabled, CancellationToken cancellationToken)
    {
        var result = await featureFlagService.ToggleAsync(key, isEnabled, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
