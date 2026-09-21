using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NToastNotify;
using OnlineConsulting.UserInterface.Areas.User.Features.UserAddress;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.UserInterface.Features.Checkout;

/// <summary>Requires login like the old controller did (no [AllowAnonymous], relies on the app-wide
/// RequireAuthenticatedUser default policy in Program.cs) - only Cart is anonymous, per the guest-cart decision.</summary>
public class CheckoutController(ICheckoutService checkoutService, IUserAddressService userAddressService, IToastNotification toastNotification, IOptions<StripeOptions> stripeOptions) : Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("/checkout/{cartId}")]
    public IActionResult Index(Guid cartId)
    {
        ViewBag.StripePublishableKey = stripeOptions.Value.PublishableKey;
        ViewBag.cartId = cartId;
        return View();
    }

    /// <summary>Places the order; a null PaymentClientSecret means the gateway already settled synchronously
    /// (e.g. Mock), so it skips straight to the order instead of the Stripe confirm step.</summary>
    [HttpPost]
    public async Task<IActionResult> PlaceOrder(Guid cartId, CancellationToken cancellationToken)
    {
        var result = await checkoutService.CreateOrderFromBasketAsync(cancellationToken);
        toastNotification.ShowResult(result);

        if (!result.IsSuccessful || result.ResultData is null)
        {
            return RedirectToAction("Index", "Checkout", new { area = "", cartId });
        }

        if (result.ResultData.PaymentClientSecret is null)
        {
            return RedirectToAction("Order", "Dashboard", new { area = "User" });
        }

        TempData["Checkout_OrderId"] = result.ResultData.OrderId.ToString();
        TempData["Checkout_OrderNumber"] = result.ResultData.OrderNumber;
        TempData["Checkout_ClientSecret"] = result.ResultData.PaymentClientSecret;
        return RedirectToAction("Confirm");
    }

    /// <summary>Renders the Stripe Payment Element for the order PlaceOrder just created; secret arrives via TempData since it can't exist before PlaceOrder runs.</summary>
    [HttpGet("/checkout/confirm")]
    public IActionResult Confirm()
    {
        if (TempData.Peek("Checkout_ClientSecret") is not string clientSecret)
        {
            return RedirectToAction("Index", "Cart", new { area = "" });
        }

        ViewBag.StripePublishableKey = stripeOptions.Value.PublishableKey;
        ViewBag.ClientSecret = clientSecret;
        ViewBag.OrderNumber = TempData.Peek("Checkout_OrderNumber") as string;
        return View();
    }

    /// <summary>Stripe's post-3DS landing page - redirect_status is client-side only; the real PaymentStatus comes from the gateway webhook, not this page.</summary>
    [HttpGet("/checkout/return")]
    public IActionResult Return(string? redirect_status)
    {
        var succeeded = string.Equals(redirect_status, "succeeded", StringComparison.OrdinalIgnoreCase);
        NToastService.Show(toastNotification,
            succeeded ? "Payment confirmed - thank you!" : "Payment is processing - we'll email you once it's confirmed.",
            succeeded ? ResultStatus.Ok : ResultStatus.Accepted);

        return RedirectToAction("Order", "Dashboard", new { area = "User" });
    }

    /// <param name="oldAddressId">Unused - the Api unmarks the previous address itself; kept only so the
    /// existing form markup keeps posting the same fields.</param>
    [HttpPost]
    public async Task<IActionResult> SetShippingAddress(Guid addressId, string oldAddressId, Guid cartId, CancellationToken cancellationToken)
    {
        var result = await userAddressService.SetShippingAsync(addressId, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index", "checkout", new { cartId });
    }

    /// <param name="oldAddressId">Unused - the Api unmarks the previous address itself; kept only so the
    /// existing form markup keeps posting the same fields.</param>
    [HttpPost]
    public async Task<IActionResult> SetBillingAddress(Guid addressId, string oldAddressId, Guid cartId, CancellationToken cancellationToken)
    {
        var result = await userAddressService.SetBillingAsync(addressId, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index", "checkout", new { cartId });
    }
}
