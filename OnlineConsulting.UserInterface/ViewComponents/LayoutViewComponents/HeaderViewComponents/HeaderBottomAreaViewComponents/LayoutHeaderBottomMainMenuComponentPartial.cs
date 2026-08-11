using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderBottomAreaViewComponents;

public class LayoutHeaderBottomMainMenuComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.BasketItemService.GetTotalBasketItemsCountAsync();

        ViewBag.TotalBasketItemsCount = result.IsSuccessful ? result.Data : 0;

        return View();
    }
}
