using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Partnership;

/// <summary>Pilot slice for the MVC -> Api migration (see ARCHITECTURE_MIGRATION.md). Deliberately thin - every
/// Api call and view-model mapping lives in IPartnershipService, this class only handles HTTP concerns (model
/// binding, ModelState, toasts, redirects). Everything for this feature (controller, service, view models, wire
/// DTO) lives in this one folder, mirroring the vertical-slice convention the Api modules already use - only the
/// .cshtml views stay under Views/ since that's a hard Razor lookup convention.</summary>
[Area("Admin")]
[Route("admin/partnerships")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class PartnershipController(IPartnershipService partnershipService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await partnershipService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public IActionResult Create() => View(new CreatePartnershipViewModel());

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreatePartnershipViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await partnershipService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await partnershipService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdatePartnershipViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await partnershipService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await partnershipService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
