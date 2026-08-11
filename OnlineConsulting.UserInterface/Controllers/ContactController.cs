using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.MessageDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Controllers;

[AllowAnonymous]
public class ContactController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMessageDto createMessageDto)
    {
        var result = await serviceManager.MessageService.AddAsync(createMessageDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index", "Home");
    }
}
