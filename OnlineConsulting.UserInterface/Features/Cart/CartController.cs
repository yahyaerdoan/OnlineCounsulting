using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Features.Cart;

/// <summary>Public cart - [AllowAnonymous] is functional here since GuestIdHandler bridges the Api's guest_id cookie, giving anonymous users real working carts.</summary>
[AllowAnonymous]
public class CartController(ICartPageService cartPageService, IToastNotification toastNotification) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await cartPageService.GetCartAsync(cancellationToken));

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("/cart/add/{slug}")]
    public async Task<IActionResult> AddToCart(string slug, CancellationToken cancellationToken)
    {
        var result = await cartPageService.AddToCartAsync(slug, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index", "Service");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveBasketItem(string id, CancellationToken cancellationToken)
    {
        var result = await cartPageService.RemoveItemAsync(Guid.Parse(id), cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index");
    }
}
