using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.UserInterface.Controllers;

[AllowAnonymous]
public class SearchController(IServiceManager serviceManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? q)
    {
        ViewBag.Query = q;

        var result = await serviceManager.ServiceService.SearchServicesAsync<ResultServiceWithImageDto>(q ?? string.Empty, false, true);

        return View(result.Data ?? []);
    }
}
