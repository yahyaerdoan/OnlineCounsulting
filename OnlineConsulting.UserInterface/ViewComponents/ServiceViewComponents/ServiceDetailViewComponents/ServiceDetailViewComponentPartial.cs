using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceDetailViewComponents;

public class ServiceDetailViewComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string slug)
    {
        var result = await serviceManager.ServiceService.GetServiceBySlugAsync<ResultServiceWithImageDto>(slug, false, true);
        if (result.Data is null)
            return Content(string.Empty);

        return View(result.Data);
    }
}
