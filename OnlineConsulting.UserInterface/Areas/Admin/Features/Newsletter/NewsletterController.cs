using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Newsletter;

[Area("Admin")]
[Route("admin/newsletter")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class NewsletterController(INewsletterService newsletterService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await newsletterService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await newsletterService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
