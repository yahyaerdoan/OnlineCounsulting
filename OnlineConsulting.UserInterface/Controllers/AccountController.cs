using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.LoginCookie;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Logout;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Register;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Controllers;

[AllowAnonymous]
public class AccountController(ISender sender, IToastNotification toastNotification, IRecaptchaService recaptchaService) : Controller
{
    [HttpGet]
    public IActionResult Register() => View();
    [HttpPost]
    public async Task<IActionResult> Register(CreateUserDto createUserDto)
    {
        var recaptchaResponse = Request.Form["g-recaptcha-response"].ToString();
        if (!await recaptchaService.VerifyAsync(recaptchaResponse))
        {
            NToastService.Show(toastNotification, "reCAPTCHA verification failed. Please try again.", ResultHandler.Core.Enums.ResultStatus.BadRequest);
            return View(createUserDto);
        }

        var result = await sender.Send(new RegisterCommand(
            createUserDto.FirstName, createUserDto.LastName, createUserDto.UserName, createUserDto.Email, createUserDto.Password));

        NToastService.Show(toastNotification, result.Title, result.Status);

        if (!result.IsSuccessful)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(createUserDto);
        }

        return RedirectToAction("Login");
    }
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        //Get LastVisitedUrl from Session if no explicit returnUrl
        returnUrl ??= HttpContext.Session.GetString("last-visited-url") ?? Url.Action("Index", "Dashboard", new { area = "user" });

        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginUserDto loginUserDto, string? returnUrl)
    {
        var result = await sender.Send(new LoginCookieCommand(loginUserDto.UserNameOrEmail, loginUserDto.Password, loginUserDto.RememberMe));

        // Error-factory results (Result.BadRequest/Forbidden/...) put the actionable message in
        // Detail and leave Title as a generic HTTP-status label ("Bad Request") - show Detail when
        // there is one so the toast doesn't just say "Bad Request" with no explanation.
        var message = result.IsSuccessful ? result.Title : (result.Detail ?? result.Title);
        NToastService.Show(toastNotification, message, result.Status, result.IsSuccessful ? "Welcome back!" : null);

        if (result.IsSuccessful)
        {
            //Fallback to user dashboard if no valid ReturnUrl
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "user" });
            }
            else
            {
                return LocalRedirect(returnUrl);
            }
        }

        ViewBag.ReturnUrl = returnUrl; // Keep it if login fails

        return View(loginUserDto);
    }
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        var result = await sender.Send(new LogoutCommand());
        NToastService.Show(toastNotification, result.Title, result.Status, "Goodbye!");
        return RedirectToAction("Index", "Home", new { area = "" });
    }
}
