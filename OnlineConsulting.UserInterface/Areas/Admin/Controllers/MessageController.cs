using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.MessageDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class MessageController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.MessageService.GetAllAsync<ResultMessageDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultMessageDto>().AsQueryable());
    }

    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.MessageService.RemoveByIdAsync(id, false);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
}
