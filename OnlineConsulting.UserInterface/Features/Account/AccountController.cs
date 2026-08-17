using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.UserInterface.Features.Account;

/// <summary>Deliberately thin - all Identity/Api orchestration lives in IAccountService, this class only handles
/// HTTP concerns (model binding, ModelState, toasts, redirects). See ARCHITECTURE_MIGRATION.md's "MVC → Api
/// Migration" entry for why this exists.</summary>
[AllowAnonymous]
public class AccountController(IAccountService accountService, IToastNotification toastNotification, IRecaptchaService recaptchaService) : Controller
{
    [HttpGet]
    public IActionResult Register() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var recaptchaResponse = Request.Form["g-recaptcha-response"].ToString();
        if (!await recaptchaService.VerifyAsync(recaptchaResponse))
        {
            NToastService.Show(toastNotification, "reCAPTCHA verification failed. Please try again.", ResultStatus.BadRequest);
            return View(model);
        }

        var result = await accountService.RegisterAsync(
            model.FirstName, model.LastName, model.UserName, model.Email, model.Password);

        NToastService.Show(toastNotification, result.Title, result.Status);

        if (!result.IsSuccessful)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);
            return View(model);
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
    {
        var (result, isAdmin) = await accountService.LoginAsync(model.UserNameOrEmail, model.Password, model.RememberMe);

        // Error-factory results put the actionable message in Detail, so prefer it over the generic Title.
        var message = result.IsSuccessful ? result.Title : (result.Detail ?? result.Title);
        NToastService.Show(toastNotification, message, result.Status, result.IsSuccessful ? "Welcome back!" : null);

        if (result.IsSuccessful)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return LocalRedirect(returnUrl);

            // No single login page per role anymore (see ARCHITECTURE_MIGRATION.md's admin-login-unification
            // entry) - everyone logs in here, the post-login destination is what differs.
            return isAdmin
                ? RedirectToAction("Index", "Dashboard", new { area = "Admin" })
                : RedirectToAction("Index", "Dashboard", new { area = "user" });
        }

        ViewBag.ReturnUrl = returnUrl; // Keep it if login fails

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        var result = await accountService.LogoutAsync();
        NToastService.Show(toastNotification, result.Title, result.Status, "Goodbye!");
        return RedirectToAction("Index", "Home", new { area = "" });
    }
}
