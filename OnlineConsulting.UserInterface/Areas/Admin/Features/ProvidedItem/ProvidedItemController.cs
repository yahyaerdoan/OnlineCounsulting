using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.ProvidedItem;

[Area("Admin")]
[Route("admin/provided-items")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class ProvidedItemController(IProvidedItemService providedItemService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await providedItemService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public IActionResult Create() => View(new CreateProvidedItemViewModel());

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateProvidedItemViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await providedItemService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await providedItemService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateProvidedItemViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await providedItemService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await providedItemService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
