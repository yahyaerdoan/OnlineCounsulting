using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Tenant;

[Area("Admin")]
[Route("admin/tenants")]
[Authorize(Policy = "RequirePlatformOwnerAccessPolicy")]
public class TenantController(ITenantService tenantService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await tenantService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var model = await tenantService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpGet("{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id, CancellationToken cancellationToken)
    {
        var result = await tenantService.SuspendAsync(id, cancellationToken);
        toastNotification.ShowResult(result);
        return RedirectToAction("Details", new { id });
    }

    [HttpGet("{id:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await tenantService.ReactivateAsync(id, cancellationToken);
        toastNotification.ShowResult(result);
        return RedirectToAction("Details", new { id });
    }

    [HttpPost("{id:guid}/modules/add")]
    public async Task<IActionResult> AddModule(Guid id, string moduleKey, CancellationToken cancellationToken)
    {
        var result = await tenantService.AddModuleAsync(id, moduleKey, cancellationToken);
        toastNotification.ShowResult(result);
        return RedirectToAction("Details", new { id });
    }

    [HttpGet("{id:guid}/modules/{moduleKey}/remove")]
    public async Task<IActionResult> RemoveModule(Guid id, string moduleKey, CancellationToken cancellationToken)
    {
        var result = await tenantService.RemoveModuleAsync(id, moduleKey, cancellationToken);
        toastNotification.ShowResult(result);
        return RedirectToAction("Details", new { id });
    }
}
