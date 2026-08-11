using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.HowIGetServiceDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class HowIGetServiceController(IServiceManager serviceManager, IToastNotification toastNotification, IDropdownDataService dropdownDataService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.HowIGetServiceService.GetAllHowIGetServicesWithImgIconsAsync<ResultHowIGetServiceDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultHowIGetServiceDto>().AsQueryable());
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.GetImgIcons = await dropdownDataService.GetImgIconsAsync();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateHowIGetServiceDto createHowIGetServiceDto)
    {
        var result = await serviceManager.HowIGetServiceService.AddAsync(createHowIGetServiceDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        ViewBag.GetImgIcons = await dropdownDataService.GetImgIconsAsync();
        var result = await serviceManager.HowIGetServiceService.GetByIdAsync<UpdateHowIGetServiceDto>(id);

        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateHowIGetServiceDto updateHowIGetServiceDto)
    {
        var result = await serviceManager.HowIGetServiceService.UpdateAsync(updateHowIGetServiceDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.HowIGetServiceService.RemoveByIdAsync(id, false);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
