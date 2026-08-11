using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.HowIGetServiceDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeHowIGetServiceViewComponents;

public class HomeHowIGetServiceComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.HowIGetServiceService.GetAllHowIGetServicesWithImgIconsAsync<ResultHowIGetServiceDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultHowIGetServiceDto>().AsQueryable());
    }
}
