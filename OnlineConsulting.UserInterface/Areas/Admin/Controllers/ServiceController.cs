using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.Extensions;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class ServiceController(IServiceManager serviceManager, IToastNotification toastNotification, IDropdownDataService dropdownDataService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.ServiceService.GetAllServicesWithImagesAsync<ResultServiceWithImageDto>(false);

        return View(result.Data ?? Enumerable.Empty<ResultServiceWithImageDto>().AsQueryable());
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.GetCategories = await dropdownDataService.GetCategoriesAsync();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceDto createServiceDto)
    {
        var result = await serviceManager.ServiceService.AddServiceWithImagesAsync(createServiceDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        ViewBag.GetCategories = await dropdownDataService.GetCategoriesAsync();
        var result = await serviceManager.ServiceService.GetServiceWithImagesByIdAsync<UpdateServiceDto>(id);

        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateServiceDto updateServiceDto)
    {
        updateServiceDto.Slug = SlugHelper.GenerateSlug(updateServiceDto.Title);

        var result = await serviceManager.ServiceService.UpdateAsync(updateServiceDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.ServiceService.RemoveServiceByIdAsync(id, false);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
