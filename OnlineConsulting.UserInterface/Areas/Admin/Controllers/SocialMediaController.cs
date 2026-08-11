using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SocialMediaDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class SocialMediaController(IServiceManager serviceManager, IToastNotification toastNotification, IDropdownDataService dropdownDataService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.SocialMediaService.GetAllSocialMediaAccontsWithIconAsync<ResultAllSocialMediaAccountsWithIconDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultAllSocialMediaAccountsWithIconDto>().AsQueryable());
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.GetClassIcons = await dropdownDataService.GetClassIconsAsync();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateSocialMediaDto createSocialMediaDto)
    {
        var result = await serviceManager.SocialMediaService.AddAsync(createSocialMediaDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        ViewBag.GetClassIcons = await dropdownDataService.GetClassIconsAsync();
        var result = await serviceManager.SocialMediaService.GetByIdAsync<UpdateSocialMediaDto>(id);

        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateSocialMediaDto updateSocialMediaDto)
    {
        var result = await serviceManager.SocialMediaService.UpdateAsync(updateSocialMediaDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.SocialMediaService.RemoveByIdAsync(id, false);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
