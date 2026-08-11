using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
[Route("Admin/{controller}/{action}/{id?}")]
public class AdminLoginController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    [HttpGet]
    public IActionResult Login() => View();
    [HttpPost]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        var result = await serviceManager.SystemUserService.LoginAdminAsync(loginUserDto);
        NToastService.Show(toastNotification, result.Title, result.Status, result.IsSuccessful ? "Welcome back!" : null);

        if (result.IsSuccessful)
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

        return RedirectToAction("Login", "Account", new { area = "" });
    }
}
