using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.UserInterface.Areas.User.ViewModels.UserViewModels;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.User.Controllers;

[Area("User")]
[Route("User/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireUserDashboardPolicy")]
public class DashboardController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    public IActionResult Index() => View();
    public IActionResult Order() => View();
    public IActionResult OrderDetail(Guid id) => View(id);
    public IActionResult Address() => View();
    public IActionResult Account() => View();
    public IActionResult Invoice(Guid id) => View(id);
    public IActionResult Download() => View();
    [HttpPost]
    public async Task<IActionResult> UpdateImageFile([FromForm] string Id, [FromForm] IFormFile image)
    {
        var result = await serviceManager.SystemUserService.UpdateUserImageAsync(Id, image);

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Account", "Dashboard", new { area = "user" });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(UserAccountViewModel model)
    {
        var result = await serviceManager.SystemUserService.ChangePasswordAsync(model.ChangePassword);

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Account", "Dashboard", new { area = "user" });
    }
}
