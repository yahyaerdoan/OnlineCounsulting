using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using ResultHandler.Core.Enums;
using ResultHandler.Implementations.Error;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.Services;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Controllers;

[AllowAnonymous]
public class AccountController(IServiceManager serviceManager, IToastNotification toastNotification, IRecaptchaService recaptchaService) : Controller
{
    [HttpGet]
    public IActionResult Register() => View();
    [HttpPost]
    public async Task<IActionResult> Register(CreateUserDto createUserDto)
    {
        var recaptchaResponse = Request.Form["g-recaptcha-response"].ToString();
        if (!await recaptchaService.VerifyAsync(recaptchaResponse))
        {
            NToastService.Show(toastNotification, "reCAPTCHA verification failed. Please try again.", ResultStatus.BadRequest);
            return View(createUserDto);
        }

        var result = await serviceManager.SystemUserService.CreateUserAsync(createUserDto);

        NToastService.Show(toastNotification, result.Title, result.Status);

        if (result is ErrorDataResult<List<ValidationRule>> ruleResult && ruleResult.Data is not null)
            AddValidationRulesToModelState(ruleResult.Data);

        if (result.IsSuccessful)
            return RedirectToAction("Login");

        return View(createUserDto);
    }
    private void AddValidationRulesToModelState(List<ValidationRule> rules)
    {
        foreach (var rule in rules)
            ModelState.AddModelError(rule.FieldName ?? string.Empty, rule.Description);
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
        var result = await serviceManager.SystemUserService.LoginUserAsync(loginUserDto);

        NToastService.Show(toastNotification, result.Title, result.Status, result.IsSuccessful ? "Welcome back!" : null);

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
        var result = await serviceManager.SystemUserService.LogoutUser();
        NToastService.Show(toastNotification, result.Title, result.Status, "Goodbye!");
        return RedirectToAction("Index", "Home", new { area = "" });
    }
}
