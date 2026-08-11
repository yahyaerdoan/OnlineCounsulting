using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class DashboardController(IServiceManager serviceManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userResult = await serviceManager.SystemUserService.GetCurrentUserAsync();

        var user = userResult.Data;

        return View(user);
    }
}
