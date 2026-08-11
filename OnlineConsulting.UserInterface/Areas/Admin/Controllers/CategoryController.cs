using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.CategoryDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class CategoryController(IServiceManager serviceManager, IToastNotification toastNotification, IDropdownDataService dropdownDataService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.CategoryService.GetAllCategoriesWithImgIconsAsync<ResultCategoryDto>(false);

        return View(result.Data ?? Enumerable.Empty<ResultCategoryDto>().AsQueryable());
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.GetImgIcons = await dropdownDataService.GetImgIconsAsync();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto createCategoryDto)
    {
        var result = await serviceManager.CategoryService.AddAsync(createCategoryDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        ViewBag.GetImgIcons = await dropdownDataService.GetImgIconsAsync();
        var result = await serviceManager.CategoryService.GetByIdAsync<UpdateCategoryDto>(id);

        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateCategoryDto updateCategoryDto)
    {
        var result = await serviceManager.CategoryService.UpdateAsync(updateCategoryDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.CategoryService.RemoveByIdAsync(id);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
