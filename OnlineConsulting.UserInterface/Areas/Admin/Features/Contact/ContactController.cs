using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Contact;

/// <summary>Admin screen for the company's contact info. Unlike the other migrated slices this has no per-id
/// Create/Delete - the Api treats /api/contact as a singleton upsert - so Index shows the current values (or an
/// empty prompt) and Update both creates and edits it.</summary>
[Area("Admin")]
[Route("admin/contact")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class ContactController(IContactService contactService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await contactService.GetAsync(cancellationToken));

    [HttpGet("edit")]
    public async Task<IActionResult> Update(CancellationToken cancellationToken) =>
        View(await contactService.GetAsync(cancellationToken) ?? new ContactViewModel());

    [HttpPost("edit")]
    public async Task<IActionResult> Update(ContactViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await contactService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }
}
