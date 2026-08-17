using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Features.Contact;

/// <summary>Public "Contact Us" page. Index just renders the page shell (the actual form and company info come
/// from ContactMessageComponentPartial/ContactInformationComponentPartial ViewComponents, out of scope for this
/// slice). Create is kept as the action name (not "Submit") because the existing ContactMessageComponentPartial
/// view posts to asp-action="Create" and that markup wasn't touched.</summary>
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
