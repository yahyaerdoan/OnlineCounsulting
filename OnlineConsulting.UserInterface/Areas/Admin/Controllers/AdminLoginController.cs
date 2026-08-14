using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.LoginAdmin;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
[Route("Admin/{controller}/{action}/{id?}")]
public class AdminLoginController(ISender sender, IToastNotification toastNotification) : Controller
{
    [HttpGet]
    public IActionResult Login() => View();
    [HttpPost]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto)
    {
        var result = await sender.Send(new LoginAdminCommand(loginUserDto.UserNameOrEmail, loginUserDto.Password, loginUserDto.RememberMe));

        var message = result.IsSuccessful ? result.Title : (result.Detail ?? result.Title);
        NToastService.Show(toastNotification, message, result.Status, result.IsSuccessful ? "Welcome back!" : null);

        if (result.IsSuccessful)
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

        return RedirectToAction("Login", "Account", new { area = "" });
    }
}
