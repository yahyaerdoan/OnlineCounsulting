using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.BreadcrumbDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.LayoutBreadcrumbViewComponents;

public class LayoutBredcrumbComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var resultData = await serviceManager.BreadcrumbService.GetAllAsync<ResultBreadcrumbDto>(false, true);
        return View(resultData.Data ?? Enumerable.Empty<ResultBreadcrumbDto>().AsQueryable());
    }
}
