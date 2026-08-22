using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ModuleOffering;

[Area("Admin")]
[Route("admin/module-offerings")]
[Authorize(Policy = "RequirePlatformOwnerAccessPolicy")]
public class ModuleOfferingController(IModuleOfferingService moduleOfferingService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await moduleOfferingService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public IActionResult Create() => View(new CreateModuleOfferingViewModel());

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateModuleOfferingViewModel createViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(createViewModel);
        }

        var result = await moduleOfferingService.CreateAsync(createViewModel, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(createViewModel);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await moduleOfferingService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateModuleOfferingViewModel updateViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(updateViewModel);
        }

        var result = await moduleOfferingService.UpdateAsync(updateViewModel, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(updateViewModel);
    }
}
