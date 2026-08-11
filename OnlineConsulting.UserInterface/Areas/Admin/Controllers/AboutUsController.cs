using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.AboutUsDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]
public class AboutUsController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.AboutUsService.GetAllAsync<ResultAboutUsDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultAboutUsDto>().AsQueryable());
    }
    [Authorize(Policy = "RequireSuperAdminPolicy")]
    public IActionResult Create() => View();
    [HttpPost]
    [Authorize(Policy = "RequireSuperAdminPolicy")]
    public async Task<IActionResult> Create(CreateAboutUsDto createAboutUsDto)
    {
        var result = await serviceManager.AboutUsService.AddAboutUsAsync(createAboutUsDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpGet]
    [Authorize(Policy = "RequireSuperAdminPolicy")]
    public async Task<IActionResult> Update(string id)
    {
        var result = await serviceManager.AboutUsService.GetByIdAsync<UpdateAboutUsDto>(id);
        await serviceManager.BasketService.CreateBasketAsync();

        return View(result.Data);
    }
    [HttpPost]
    [Authorize(Policy = "RequireSuperAdminPolicy")]
    public async Task<IActionResult> Update(UpdateAboutUsDto updatePartnershipDto)
    {
        var result = await serviceManager.AboutUsService.UpdateAsync(updatePartnershipDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpPost]
    [Authorize(Policy = "RequireSuperAdminPolicy")]
    public async Task<IActionResult> UpdateImageFile([FromForm] string Id, [FromForm] IFormFile Image)
    {
        var result = await serviceManager.AboutUsService.UpdateAboutUsImageAsync(Id, Image);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
}
