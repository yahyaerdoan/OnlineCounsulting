using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.GalleryItem;

[Area("Admin")]
[Route("admin/gallery-items")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class GalleryItemController(IAdminGalleryItemService galleryItemService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await galleryItemService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await galleryItemService.BuildCreateModelAsync(cancellationToken));

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateGalleryItemViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await galleryItemService.FillCategoriesAsync(model, cancellationToken);
            return View(model);
        }

        var result = await galleryItemService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        if (result.IsSuccessful)
            return RedirectToAction("Index");

        await galleryItemService.FillCategoriesAsync(model, cancellationToken);
        return View(model);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await galleryItemService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateGalleryItemViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await galleryItemService.FillCategoriesAsync(model, cancellationToken);
            return View(model);
        }

        var result = await galleryItemService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        if (result.IsSuccessful)
            return RedirectToAction("Index");

        await galleryItemService.FillCategoriesAsync(model, cancellationToken);
        return View(model);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await galleryItemService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
