using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.FooterAboutDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.FooterViewComponents.FooterTopAreaViewComponents;

public class LayoutFooterTopAreaBlock1ViewComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.FooterAboutService.GetAllAsync<ResultFooterAboutDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultFooterAboutDto>().AsQueryable());
    }
}
