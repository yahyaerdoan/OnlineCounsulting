using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NToastNotify;
using OnlineConsulting.UserInterface.Features.Checkout;
using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Membership;

/// <summary>Requires login (no [AllowAnonymous], app-wide RequireAuthenticatedUser default policy) - the
/// public plan browsing page (Features/Membership, top-level) is what's anonymous, this is the paid-subscribe
/// step. Mirrors CheckoutController's PRG+TempData pattern for carrying a Stripe ClientSecret across the
/// redirect to the confirm step, but the card-tokenization step happens earlier here (Subscribe POST needs a
/// PaymentMethodId already in hand - Checkout's Order doesn't need one until Confirm).</summary>
[Area("User")]
[Route("user/membership")]
public class MembershipController(IMembershipService membershipService, IToastNotification toastNotification, IOptions<StripeOptions> stripeOptions) : Controller
{
    [HttpGet("subscribe/{planId:guid}")]
    public async Task<IActionResult> Subscribe(Guid planId, CancellationToken cancellationToken)
    {
        var plan = await membershipService.GetPlanAsync(planId, cancellationToken);
        if (plan is null)
        {
            return NotFound();
        }

        ViewBag.StripePublishableKey = stripeOptions.Value.PublishableKey;

        return View(new SubscribeMembershipViewModel
        {
            PlanId = plan.Id,
            PlanName = plan.Name,
            BillingCycle = plan.BillingCycle,
            Price = plan.Price,
            CreditBalance = await membershipService.GetCreditBalanceAsync(cancellationToken),
        });
    }

    [HttpPost("subscribe")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(SubscribeMembershipViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var plan = await membershipService.GetPlanAsync(model.PlanId, cancellationToken);
            ViewBag.StripePublishableKey = stripeOptions.Value.PublishableKey;
            model.PlanName = plan?.Name ?? model.PlanName;
            model.BillingCycle = plan?.BillingCycle ?? model.BillingCycle;
            model.Price = plan?.Price ?? model.Price;
            return View(model);
        }

        decimal? creditToApply = model.ApplyCredit && model.CreditBalance > 0 ? model.CreditBalance : null;
        var result = await membershipService.SubscribeAsync(model.PlanId, model.PaymentMethodId, creditToApply, cancellationToken);

        if (!result.IsSuccessful || result.ResultData is null)
        {
            toastNotification.ShowResult(result.WithoutData());
            return RedirectToAction("Subscribe", new { planId = model.PlanId });
        }

        // A null ClientSecret means the subscription's first invoice already settled synchronously (e.g. Mock
        // gateway, or a card that didn't need 3DS/SCA) - nothing left for the client to confirm.
        if (result.ResultData.ClientSecret is null)
        {
            NToastService.Show(toastNotification, "Subscribed successfully!", ResultStatus.Ok);
            return RedirectToAction("Membership", "Dashboard", new { area = "User" });
        }

        TempData["Membership_ClientSecret"] = result.ResultData.ClientSecret;
        return RedirectToAction("Confirm");
    }

    [HttpGet("confirm")]
    public IActionResult Confirm()
    {
        if (TempData.Peek("Membership_ClientSecret") is not string clientSecret)
        {
            return RedirectToAction("Index", "Membership", new { area = "" });
        }

        ViewBag.StripePublishableKey = stripeOptions.Value.PublishableKey;
        ViewBag.ClientSecret = clientSecret;
        return View();
    }

    [HttpGet("return")]
    public IActionResult Return(string? status)
    {
        var succeeded = !string.Equals(status, "failed", StringComparison.OrdinalIgnoreCase);
        NToastService.Show(toastNotification,
            succeeded ? "Membership confirmed - thank you!" : "Payment could not be confirmed. Please try again.",
            succeeded ? ResultStatus.Ok : ResultStatus.BadRequest);

        return RedirectToAction("Membership", "Dashboard", new { area = "User" });
    }

    [HttpPost("cancel")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(CancellationToken cancellationToken)
    {
        var result = await membershipService.CancelAsync(cancellationToken);
        toastNotification.ShowResult(result);

        return RedirectToAction("Membership", "Dashboard", new { area = "User" });
    }
}
