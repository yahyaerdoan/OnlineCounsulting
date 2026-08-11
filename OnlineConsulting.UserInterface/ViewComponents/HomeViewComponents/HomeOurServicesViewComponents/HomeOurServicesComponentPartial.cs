using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeOurServicesViewComponents;

public class HomeOurServicesComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.ServiceService.GetAllServicesByFeaturedAreaTrueAsync<ResultServiceWithImageDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultServiceWithImageDto>().AsQueryable());
    }
}
