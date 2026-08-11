using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceImageDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class ServiceImageController(IServiceManager serviceManger, IToastNotification toastNotification) : Controller
{
    //public IActionResult Index()
    //{
    //    var result = serviceManger.ServiceImageService.GetAll<ResultServiceImageDto>();
    //    return View(result.Data);
    //}
    public IActionResult Create() => View();
    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceImageDto createServiceImageDto)
    {
        var result = await serviceManger.ServiceImageService.AddServiceImageAsync(createServiceImageDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index", "Service");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        var result = await serviceManger.ServiceImageService.GetByIdAsync<UpdateServiceImageDto>(id);
        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateServiceImageDto updateServiceImageDto)
    {
        var result = await serviceManger.ServiceImageService.UpdateAsync(updateServiceImageDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(string id, string modelId)
    {
        var result = await serviceManger.ServiceImageService.RemoveServiceImageByIdAsync(id, false);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Update", "Service", new { id = modelId });
    }
    [HttpPost]
    public async Task<IActionResult> UpdateImageFile([FromForm] string Id, [FromForm] IFormFile Image)
    {
        var result = await serviceManger.ServiceImageService.UpdateServiceImageAsync(Id, Image);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
}
