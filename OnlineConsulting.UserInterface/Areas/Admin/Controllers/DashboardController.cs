using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.UserInterface.Common;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class DashboardController(ISender sender) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userResult = await sender.Send(new GetCurrentUserQuery());

        var user = userResult.Data?.ToResultUserDto();

        return View(user);
    }
}
