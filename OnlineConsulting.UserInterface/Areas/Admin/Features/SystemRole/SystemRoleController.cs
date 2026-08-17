using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemRole;

[Area("Admin")]
[Route("admin/system-roles")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class SystemRoleController(ISystemRoleService systemRoleService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await systemRoleService.GetAllAsync(cancellationToken));

    [HttpGet("create")]
    public IActionResult Create() => View(new CreateSystemRoleViewModel());

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateSystemRoleViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await systemRoleService.CreateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
    {
        var model = await systemRoleService.GetByIdAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> Update(UpdateSystemRoleViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await systemRoleService.UpdateAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return result.IsSuccessful ? RedirectToAction("Index") : View(model);
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await systemRoleService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
