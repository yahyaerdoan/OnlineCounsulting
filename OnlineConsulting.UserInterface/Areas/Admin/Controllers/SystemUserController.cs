using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class SystemUserController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.SystemUserService.GetAllUsersAsync();

        return View(result.Data ?? new List<ResultUserDto>());
    }
    [HttpGet]
    public async Task<IActionResult> AssingARoleToUser(string id)
    {
        ViewBag.UserId = id;
        var result = await serviceManager.SystemUserService.GetUserRolesAsync(id);

        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> AssingARoleToUser(string id, List<AssingARoleToUserDto> assingARoleToUserDtos)
    {
        var result = await serviceManager.SystemUserService.AssingARoleToUserAsync(id, assingARoleToUserDtos);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.SystemUserService.DeleteSytemUserByIdAsync(id);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
