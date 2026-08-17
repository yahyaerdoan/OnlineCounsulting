using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.SystemUser;

[Area("Admin")]
[Route("admin/system-users")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]
public class SystemUserController(ISystemUserService systemUserService, IToastNotification toastNotification) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await systemUserService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}/roles")]
    public async Task<IActionResult> AssingARoleToUser(Guid id, CancellationToken cancellationToken)
    {
        ViewBag.UserId = id;
        return View(await systemUserService.GetUserRolesAsync(id, cancellationToken));
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AssingARoleToUser(Guid id, List<RoleAssignmentViewModel> assingARoleToUserDtos, CancellationToken cancellationToken)
    {
        var result = await systemUserService.AssignRolesAsync(id, assingARoleToUserDtos, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }

    [HttpGet("{id:guid}/delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await systemUserService.DeleteAsync(id, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
