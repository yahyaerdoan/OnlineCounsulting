using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.User.Features.Dashboard;
using OnlineConsulting.UserInterface.Common;
using OnlineConsulting.UserInterface.Features.Cart;
using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardUpperInfoViewComponents;

/// <summary>Dashboard layout header. Kept out of a feature folder because it is a layout widget, not a slice.</summary>
public class DashboardUpperInfoComponentPartial(ICartService cartService, IApiClient apiClient) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await apiClient.GetAsync<CurrentUserResponse>("/api/users/me");
        if (!userResult.IsSuccessful || userResult.ResultData is null)
        {
            return Content(string.Empty);
        }

        ViewBag.TotalBasketItemsCount = await cartService.GetItemsCountAsync();

        var viewModel = new UserAccountViewModel
        {
            User = userResult.ResultData.ToUserSummaryViewModel()
        };

        return View(viewModel);
    }
}
