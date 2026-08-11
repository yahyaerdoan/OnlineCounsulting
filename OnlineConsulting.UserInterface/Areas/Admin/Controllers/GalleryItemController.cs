using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryItemDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class GalleryItemController(IServiceManager serviceManager, IToastNotification toastNotification, IDropdownDataService dropdownDataService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.GalleryItemService.GetAllGalleryItemsAsync<ResultGalleryItemWithCategoriesDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultGalleryItemWithCategoriesDto>().AsQueryable());
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.GetGalleryCategories = await dropdownDataService.GetGalleryCategoriesAsync();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateGalleryItemDto createGalleryItemDto)
    {
        var result = await serviceManager.GalleryItemService.AddGalleryItemAsync(createGalleryItemDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        var result = serviceManager.GalleryItemService.GetByIdGalleryItem<UpdateGalleryItemWithCategoryDto>(id, false);
        ViewBag.GetGalleryCategories = await dropdownDataService.GetGalleryCategoriesAsync();

        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateGalleryItemWithCategoryDto updateGalleryItemDto)
    {
        var result = await serviceManager.GalleryItemService.UpdateGalleryItemAsync(updateGalleryItemDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.GalleryItemService.RemoveGalleryItemByIdAsync(id);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
