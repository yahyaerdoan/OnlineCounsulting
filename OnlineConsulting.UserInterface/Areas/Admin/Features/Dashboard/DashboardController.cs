using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Common;
using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Dashboard;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminAreaAccessPolicy")]

public class DashboardController(IApiClient apiClient) : Controller
{
    public async Task<IActionResult> Index()
    {
        var userResult = await apiClient.GetAsync<CurrentUserResponse>("/api/users/me");

        var user = userResult.ResultData?.ToUserSummaryViewModel();

        return View(user);
    }
}
