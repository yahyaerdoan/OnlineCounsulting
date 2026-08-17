using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.Admin.Features.ProvidedItem;

namespace OnlineConsulting.UserInterface.ViewComponents.HomeViewComponents.HomeWhatWeProvideViewComponents;

public class HomeWhatWeProvideProvidedItemComponentPartial(IProvidedItemService providedItemService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await providedItemService.GetAllAsync();
        return View(result);
    }
}
