using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.MessageDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Controllers;

[AllowAnonymous]
public class ServiceController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    private const string _serviceDetailRoute = "/service/detail/{slug}";
    [HttpGet]
    public IActionResult Index(int size = 6, int page = 1)
    {
        ViewBag.size = size;
        ViewBag.page = page;
        return View();
    }
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet(_serviceDetailRoute)]
    public IActionResult Detail() => View();
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost(_serviceDetailRoute)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Detail(string slug, CreateMessageDto createMessageDto)
    {
        var result = await serviceManager.MessageService.AddAsync(createMessageDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Detail", "Service", new { slug });
    }
}
