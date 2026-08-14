using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.Modules.Identity.Application.Features.Users.ChangePassword;
using OnlineConsulting.Modules.Identity.Application.Features.Users.UpdateUserImage;
using OnlineConsulting.UserInterface.Areas.User.ViewModels.UserViewModels;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.User.Controllers;

[Area("User")]
[Route("User/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireUserDashboardPolicy")]
public class DashboardController(ISender sender, IToastNotification toastNotification) : Controller
{
    public IActionResult Index() => View();
    public IActionResult Order() => View();
    public IActionResult OrderDetail(Guid id) => View(id);
    public IActionResult Address() => View();
    public IActionResult Account() => View();
    public IActionResult Invoice(Guid id) => View(id);
    public IActionResult Download() => View();
    [HttpPost]
    public async Task<IActionResult> UpdateImageFile([FromForm] Guid Id, [FromForm] IFormFile image)
    {
        var result = await sender.Send(new UpdateUserImageCommand(Id, image));

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Account", "Dashboard", new { area = "user" });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(UserAccountViewModel model)
    {
        var result = await sender.Send(new ChangePasswordCommand(model.ChangePassword.Id, model.ChangePassword.CurrentPassword, model.ChangePassword.NewPassword));

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Account", "Dashboard", new { area = "user" });
    }
}
