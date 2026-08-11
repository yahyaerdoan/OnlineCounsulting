using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.UserInterface.Areas.User.ViewModels.UserViewModels;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardUpperInfoViewComponents;

public class DashboardUpperInfoComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await serviceManager.SystemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful || userResult.Data is null)
            return Content(string.Empty);

        var totalCount = await serviceManager.BasketItemService.GetTotalBasketItemsCountAsync();

        ViewBag.TotalBasketItemsCount = totalCount.Data;

        var viewModel = new UserAccountViewModel
        {
            User = userResult.Data
        };

        return View(viewModel);
    }
}
