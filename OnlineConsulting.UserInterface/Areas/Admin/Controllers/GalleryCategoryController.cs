using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.GalleryCategoryDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class GalleryCategoryController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.GalleryCategoryService.GetAllAsync<ResultGalleryCategoryDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultGalleryCategoryDto>().AsQueryable());
    }
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    public async Task<IActionResult> Create(CreateGalleryCategoryDto createGalleryCategoryDto)
    {
        var result = await serviceManager.GalleryCategoryService.AddAsync(createGalleryCategoryDto);

        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        var result = await serviceManager.GalleryCategoryService.GetByIdAsync<UpdateGalleryCategoryDto>(id);
        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateGalleryCategoryDto updateGalleryCategoryDto)
    {
        var result = await serviceManager.GalleryCategoryService.UpdateAsync(updateGalleryCategoryDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.GalleryCategoryService.RemoveByIdAsync(id);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
}
