using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ProvidedItemDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class ProvidedItemController(IServiceManager serviceManager, IToastNotification toastNotification, IDropdownDataService dropdownDataService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.ProvidedItemService.GetAllProvidedItemsWithImgIconsAsync<ResultProvidedItemDto>();

        return View(result.Data ?? Enumerable.Empty<ResultProvidedItemDto>().AsQueryable());
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.GetImgIcons = await dropdownDataService.GetImgIconsAsync();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateProvidedItemDto createProvidedItemDto)
    {
        var result = await serviceManager.ProvidedItemService.AddAsync(createProvidedItemDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        ViewBag.GetImgIcons = await dropdownDataService.GetImgIconsAsync();
        var result = await serviceManager.ProvidedItemService.GetByIdAsync<UpdateProvidedItemDto>(id);

        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateProvidedItemDto updateProvidedItemDto)
    {
        var result = await serviceManager.ProvidedItemService.UpdateAsync(updateProvidedItemDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.ProvidedItemService.RemoveByIdAsync(id);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
