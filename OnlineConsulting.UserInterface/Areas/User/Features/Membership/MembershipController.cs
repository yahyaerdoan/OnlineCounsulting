using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NToastNotify;
using OnlineConsulting.UserInterface.Features.Checkout;
using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;
using ResultHandler.Core.Enums;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Membership;

/// <summary>Requires login (paid-subscribe step); mirrors CheckoutController's PRG+TempData pattern but needs a PaymentMethodId already in hand at Subscribe POST, not just at Confirm.</summary>
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

    /// <summary>Subscribes to a plan; a null ClientSecret in the result means the first invoice already settled synchronously (e.g. Mock gateway, or a card that skipped 3DS/SCA), so no client-side confirm step is needed.</summary>
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
