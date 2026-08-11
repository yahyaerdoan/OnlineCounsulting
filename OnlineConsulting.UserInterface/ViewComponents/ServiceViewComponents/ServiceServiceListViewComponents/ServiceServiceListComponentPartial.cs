using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.ServiceViewComponents.ServiceServiceListViewComponents;

public class ServiceServiceListComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(int size, int page)
    {
        var result = await serviceManager.ServiceService.GetAllServicesWithPagedAsync(size, page, false, true);
        return View(result.Data ?? new ResultServiceWithPagedDto { Size = size, Page = page });
    }
}
