using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Partnership;

/// <summary>Pilot slice for the MVC -> Api migration (see ARCHITECTURE_MIGRATION.md) - vertical-slice folder like the Api modules, only .cshtml stays under Views/ per Razor's lookup convention.</summary>
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
