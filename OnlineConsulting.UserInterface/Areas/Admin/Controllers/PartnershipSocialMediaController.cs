using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IDropdownServices;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipSocialMediaDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class PartnershipSocialMediaController(IServiceManager serviceManager, IToastNotification toastNotification, IDropdownDataService dropdownDataService) : Controller
{
    public async Task<IActionResult> Index(string id)
    {
        ViewBag.PartnershipId = id;
        var result = await serviceManager.PartnershipSocialMediaService.GetAllSocialMediasByParnershipIdAsync<ResultPartnershipSocialMediaDto>(id, false);

        return View(result.Data ?? Enumerable.Empty<ResultPartnershipSocialMediaDto>().AsQueryable());
    }
    [HttpGet]
    public async Task<IActionResult> Create(string id)
    {
        ViewBag.PartnershipId = id;
        ViewBag.GetClassIcons = await dropdownDataService.GetClassIconsAsync();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreatePartnershipSocialMediaDto createPartnershipSocialMediaDto)
    {
        var result = await serviceManager.PartnershipSocialMediaService.AddAsync(createPartnershipSocialMediaDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index", "Partnership");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id, string partnershipId)
    {
        ViewBag.PartnershipId = Guid.Parse(partnershipId);
        ViewBag.GetClassIcons = await dropdownDataService.GetClassIconsAsync();
        var result = await serviceManager.PartnershipSocialMediaService.GetByIdAsync<UpdatePartnershipSocialMediaDto>(id);

        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdatePartnershipSocialMediaDto updatePartnershipSocialMediaDto)
    {
        var result = await serviceManager.PartnershipSocialMediaService.UpdateAsync(updatePartnershipSocialMediaDto);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index", "Partnership");
    }

    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.PartnershipSocialMediaService.RemoveByIdAsync(id);
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
