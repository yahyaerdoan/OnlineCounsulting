using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Cart;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderBottomAreaViewComponents;

public class LayoutHeaderBottomMainMenuComponentPartial(ICartService cartService) : ViewComponent
{
    /// <summary>Renders for every visitor, including anonymous ones - cart item count works via the guest_id cookie bridged by GuestIdHandler.</summary>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        ViewBag.TotalBasketItemsCount = await cartService.GetItemsCountAsync();

        return View();
    }
}
