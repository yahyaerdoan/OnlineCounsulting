using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.ProvidedItemDtos;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeWhatWeProvideViewComponents;

public class HomeWhatWeProvideProvidedItemComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.ProvidedItemService.GetAllAsync<ResultProvidedItemDto>(false, true);
        return View(result.Data ?? Enumerable.Empty<ResultProvidedItemDto>().AsQueryable());
    }
}
