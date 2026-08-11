using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.PartnershipDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class PartnershipController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.PartnershipService.GetAllAsync<ResultPartnershipDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultPartnershipDto>().AsQueryable());
    }
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    public async Task<IActionResult> Create(CreatePartnershipDto createPartnershipDto)
    {
        var result = await serviceManager.PartnershipService.AddPartnershipAsync(createPartnershipDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        var result = await serviceManager.PartnershipService.GetByIdAsync<UpdatePartnershipDto>(id);
        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdatePartnershipDto updatePartnershipDto)
    {
        var result = await serviceManager.PartnershipService.UpdateAsync(updatePartnershipDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(string id)
    {
        var result = await serviceManager.PartnershipService.RemovePartnershipByIdAsync(id);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> UpdateImageFile([FromForm] string Id, [FromForm] IFormFile Image)
    {
        var result = await serviceManager.PartnershipService.UpdatePartnershipImageAsync(Id, Image);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
}
