using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Service;

/// <summary>Also owns what used to be a separate ServiceImageController - see the media-item actions at the bottom.</summary>
[Area("Admin")]
[Route("admin/services")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class ServiceController(IAdminServiceCatalogService serviceCatalogService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await serviceCatalogService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await serviceCatalogService.BuildCreateModelAsync(cancellationToken));

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateServiceViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await serviceCatalogService.FillCategoriesAsync(model, cancellationToken);
            return View(model);
        }

        var result = await serviceCatalogService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        if (result.IsSuccessful)
        {
            return RedirectToAction("Index");
        }

        await serviceCatalogService.FillCategoriesAsync(model, cancellationToken);
        return View(model);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await serviceCatalogService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateServiceViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await serviceCatalogService.FillCategoriesAsync(model, cancellationToken);
            return View(model);
        }

        var result = await serviceCatalogService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        if (result.IsSuccessful)
        {
            return RedirectToAction("Index");
        }

        await serviceCatalogService.FillCategoriesAsync(model, cancellationToken);
        return View(model);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await serviceCatalogService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }

    [HttpPost("{id:guid}/media-items")]
    public async Task<IActionResult> AddImage(Guid id, IFormFile? image, CancellationToken cancellationToken)
    {
        var result = await serviceCatalogService.AddImageAsync(id, image, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Update", new { id });
    }

    [HttpPost("{id:guid}/media-items/{imageId:guid}/delete")]
    public async Task<IActionResult> RemoveImage(Guid id, Guid imageId, CancellationToken cancellationToken)
    {
        var result = await serviceCatalogService.RemoveImageAsync(id, imageId, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Update", new { id });
    }
}
