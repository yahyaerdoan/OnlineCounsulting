using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Features.Cart;

namespace OnlineConsulting.UserInterface.ViewComponents.CheckoutViewComponents.CheckoutOrderViewComponents;

public class CheckoutOrderComponentPartial(ICartPageService cartPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Guid cartId)
    {
        var cart = await cartPageService.GetCartAsync();
        return View(cart?.Items ?? []);
    }
}
