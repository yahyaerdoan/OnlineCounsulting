using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.WhatWeProvideDtos;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class WhatWeProvideController(IServiceManager serviceManager, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await serviceManager.WhatWeProvideService.GetAllAsync<ResultWhatWeProvideDto>(false);
        return View(result.Data ?? Enumerable.Empty<ResultWhatWeProvideDto>().AsQueryable());
    }
    public IActionResult Create() => View();
    [HttpPost]
    public async Task<IActionResult> Create(CreateWhatWeProvideDto createWhatWeProvideDto)
    {
        var result = await serviceManager.WhatWeProvideService.AddWhatWeProvideAsync(createWhatWeProvideDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(string id)
    {
        var result = await serviceManager.WhatWeProvideService.GetByIdAsync<UpdateWhatWeProvideDto>(id);
        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateWhatWeProvideDto updateWhatWeProvideDto)
    {
        var result = await serviceManager.WhatWeProvideService.UpdateAsync(updateWhatWeProvideDto);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> UpdateImageFile([FromForm] string Id, [FromForm] IFormFile Image)
    {
        var result = await serviceManager.WhatWeProvideService.UpdateWhatWeProvideImageAsync(Id, Image);
        NToastService.Show(toastNotification, result.Title, result.Status);
        return RedirectToAction("Index");
    }
}
