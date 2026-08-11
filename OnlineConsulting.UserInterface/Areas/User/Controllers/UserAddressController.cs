using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserAddressDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.User.Controllers;

[Area("User")]
[Route("User/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireUserDashboardPolicy")]
public class UserAddressController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserAddressDto createUserAddressDto)
    {
        var result = await serviceManager.UserAddressService.AddUserAddressAsync(createUserAddressDto);

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Address", "Dashboard", new { area = "user" });
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateUserAddressDto updateUserAddressDto)
    {
        var result = await serviceManager.UserAddressService.UpdateUserAddressAsync(updateUserAddressDto);

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Address", "Dashboard", new { area = "user" });
    }

    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.UserAddressService.RemoveByIdAsync(id);

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Address", "Dashboard", new { area = "user" });
    }
}
