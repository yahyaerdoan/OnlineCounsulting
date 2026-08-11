using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ServiceDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.FooterViewComponents.FooterTopAreaViewComponents;

public class LayoutFooterTopAreaBlock3ViewComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.ServiceService.GetAllServicesWithImagesAsync<ResultServiceWithImageDto>(false, true);
        return View((result.Data ?? Enumerable.Empty<ResultServiceWithImageDto>().AsQueryable()).OrderByDescending(x => x.CreatedDate).Take(3));
    }
}
