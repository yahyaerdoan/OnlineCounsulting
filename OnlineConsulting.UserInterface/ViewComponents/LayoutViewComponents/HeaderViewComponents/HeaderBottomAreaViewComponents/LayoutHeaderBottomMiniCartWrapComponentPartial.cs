using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Cart;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderBottomAreaViewComponents;

public class LayoutHeaderBottomMiniCartWrapComponentPartial(ICartPageService cartPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cart = await cartPageService.GetCartAsync();
        return View(cart);
    }
}
