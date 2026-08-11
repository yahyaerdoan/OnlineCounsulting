using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SliderItemDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeSliderViewComponents;

public class HomeSliderComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.SliderItemService.GetAllAsync<ResultSliderItemDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultSliderItemDto>().AsQueryable());
    }
}
