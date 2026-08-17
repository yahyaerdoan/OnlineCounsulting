using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Cart;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderBottomAreaViewComponents;

public class LayoutHeaderBottomMainMenuComponentPartial(ICartService cartService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        // Renders on every page for every visitor including anonymous ones - ICartService/IApiClient already
        // bridge the Api's guest_id cookie transparently via GuestIdHandler, so no logged-in user is assumed here.
        ViewBag.TotalBasketItemsCount = await cartService.GetItemsCountAsync();

        return View();
    }
}
