using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FooterAboutDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class FooterAboutController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.FooterAboutService.GetAllAsync<ResultFooterAboutDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultFooterAboutDto>().AsQueryable());
    }
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    public async Task<IActionResult> Create(CreateFooterAboutDto createFooterAboutDto)
    {
        var result = await serviceManager.FooterAboutService.AddFooterAboutAsync(createFooterAboutDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        var result = await serviceManager.FooterAboutService.GetByIdAsync<UpdateFooterAboutDto>(id);
        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateFooterAboutDto updateFooterAboutDto)
    {
        var result = await serviceManager.FooterAboutService.UpdateAsync(updateFooterAboutDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.FooterAboutService.RemoveFooterAboutByIdAsync(id, false);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> UpdateImageFile([FromForm] string Id, [FromForm] IFormFile Image)
    {
        var result = await serviceManager.FooterAboutService.UpdateFooterAboutImageAsync(Id, Image);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
}
