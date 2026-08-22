using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Bundle;

[Area("Admin")]
[Route("admin/bundles")]
[Authorize(Policy = "RequirePlatformOwnerAccessPolicy")]
public class BundleController(IBundleService bundleService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await bundleService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await bundleService.GetCreateFormAsync(cancellationToken));

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateBundleViewModel createViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            createViewModel.AvailableModuleKeys = (await bundleService.GetCreateFormAsync(cancellationToken)).AvailableModuleKeys;
            return View(createViewModel);
        }

        var result = await bundleService.CreateAsync(createViewModel, cancellationToken);
        toastNotification.ShowResult(result);

        if (result.IsSuccessful)
        {
            return RedirectToAction("Index");
        }

        createViewModel.AvailableModuleKeys = (await bundleService.GetCreateFormAsync(cancellationToken)).AvailableModuleKeys;
        return View(createViewModel);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await bundleService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateBundleViewModel updateViewModel, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var reloaded = await bundleService.GetByIdAsync(updateViewModel.Id, cancellationToken);
            updateViewModel.AvailableModuleKeys = reloaded?.AvailableModuleKeys ?? [];
            return View(updateViewModel);
        }

        var result = await bundleService.UpdateAsync(updateViewModel, cancellationToken);
        toastNotification.ShowResult(result);

        if (result.IsSuccessful)
        {
            return RedirectToAction("Index");
        }

        var reloadedForError = await bundleService.GetByIdAsync(updateViewModel.Id, cancellationToken);
        updateViewModel.AvailableModuleKeys = reloadedForError?.AvailableModuleKeys ?? [];
        return View(updateViewModel);
    }
}
