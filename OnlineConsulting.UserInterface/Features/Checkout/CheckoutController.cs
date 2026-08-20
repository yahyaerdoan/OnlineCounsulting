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

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(Guid cartId, CancellationToken cancellationToken)
    {
        var result = await checkoutService.CreateOrderFromBasketAsync(cancellationToken);
        toastNotification.ShowResult(result);

        if (!result.IsSuccessful || result.ResultData is null)
        {
            return RedirectToAction("Index", "Checkout", new { area = "", cartId });
        }

        // A null PaymentClientSecret means the gateway already settled the payment synchronously (e.g. Mock) -
        // nothing left for the client to confirm, go straight to the order. A real gateway like Stripe leaves
        // the order Pending until the client completes 3DS/SCA, so route through the confirmation page instead.
        if (result.ResultData.PaymentClientSecret is null)
        {
            return RedirectToAction("Order", "Dashboard", new { area = "User" });
        }

        TempData["Checkout_OrderId"] = result.ResultData.OrderId.ToString();
        TempData["Checkout_OrderNumber"] = result.ResultData.OrderNumber;
        TempData["Checkout_ClientSecret"] = result.ResultData.PaymentClientSecret;
        return RedirectToAction("Confirm");
    }

    /// <summary>Renders the Stripe Payment Element for the order just created by PlaceOrder - the client secret
    /// only exists once an Order row does, so this can't be shown before PlaceOrder runs (hence the TempData
    /// round-trip via the PRG redirect rather than passing it as a query string).</summary>
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

    /// <summary>Stripe redirects the browser here after the customer completes (or abandons) 3DS/SCA - the query
    /// params it appends (redirect_status etc.) only describe what just happened client-side. The order's real
    /// PaymentStatus comes from the gateway's webhook (already wired, see Modules/Commerce's OnPaymentStatusChanged),
    /// so this is just a friendly landing page, not the source of truth.</summary>
    [HttpGet("/checkout/return")]
    public IActionResult Return(string? redirect_status)
    {
        var succeeded = string.Equals(redirect_status, "succeeded", StringComparison.OrdinalIgnoreCase);
        NToastService.Show(toastNotification,
            succeeded ? "Payment confirmed - thank you!" : "Payment is processing - we'll email you once it's confirmed.",
            succeeded ? ResultStatus.Ok : ResultStatus.Accepted);

        return RedirectToAction("Order", "Dashboard", new { area = "User" });
    }

    // oldAddressId is no longer needed - the Api unmarks the previous shipping address itself, kept as a route
    // parameter only so the untouched CheckoutBillingDetailsComponentPartial form markup keeps posting the same fields.
    [HttpPost]
    public async Task<IActionResult> SetShippingAddress(Guid addressId, string oldAddressId, Guid cartId, CancellationToken cancellationToken)
    {
        var result = await userAddressService.SetShippingAsync(addressId, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index", "checkout", new { cartId });
    }

    [HttpPost]
    public async Task<IActionResult> SetBillingAddress(Guid addressId, string oldAddressId, Guid cartId, CancellationToken cancellationToken)
    {
        var result = await userAddressService.SetBillingAsync(addressId, cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Index", "checkout", new { cartId });
    }
}
