using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ClassIconDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class ClassIconController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.ClassIconService.GetAllAsync<ResultClassIconDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultClassIconDto>().AsQueryable());
    }
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    public async Task<IActionResult> Create(CreateClassIconDto createClassIconDto)
    {
        var result = await serviceManager.ClassIconService.AddAsync(createClassIconDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        var result = await serviceManager.ClassIconService.GetByIdAsync<UpdateClassIconDto>(id);
        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateClassIconDto updateClassIconDto)
    {
        var result = await serviceManager.ClassIconService.UpdateAsync(updateClassIconDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.ClassIconService.RemoveByIdAsync(id);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
}
