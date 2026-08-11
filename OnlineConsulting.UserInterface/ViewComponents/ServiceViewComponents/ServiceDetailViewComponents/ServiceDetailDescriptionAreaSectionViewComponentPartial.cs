using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceDetailViewComponents;

public class ServiceDetailDescriptionAreaSectionViewComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public IViewComponentResult Invoke(string slug)
    {
        var result = serviceManager.ServiceService.GetServiceBySlugAsync<ResultServiceWithImageDto>(slug, false, true);
        return View(result.Result.Data);
    }
}
