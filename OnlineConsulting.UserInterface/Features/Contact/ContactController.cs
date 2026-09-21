using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Features.Contact;

/// <summary>Public "Contact Us" page shell; Create is kept as the action name since existing markup posts to asp-action="Create".</summary>
[AllowAnonymous]
public class ContactController(IContactService contactService, IToastNotification toastNotification) : Controller
{
    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMessageViewModel model, CancellationToken cancellationToken)
    {
        var result = await contactService.SubmitMessageAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index", "Home");
    }
}
