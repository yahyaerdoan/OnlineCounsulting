using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.Features.Contact;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Features.Service;

/// <summary>Public service catalog/detail pages; the "ask about this service" form reuses IContactService's generic inquiries endpoint, no ServiceId attached.</summary>
[AllowAnonymous]
public class ServiceController(IContactService contactService, IToastNotification toastNotification) : Controller
{
    private const string ServiceDetailRoute = "/service/detail/{slug}";

    [HttpGet]
    public IActionResult Index(int size = 6, int page = 1)
    {
        ViewBag.size = size;
        ViewBag.page = page;
        return View();
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet(ServiceDetailRoute)]
    public IActionResult Detail() => View();

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost(ServiceDetailRoute)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Detail(string slug, CreateMessageViewModel model, CancellationToken cancellationToken)
    {
        var result = await contactService.SubmitMessageAsync(model, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Detail", "Service", new { slug });
    }
}
