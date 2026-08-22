using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Membership;

public class DashboardMembershipComponentPartial(IMembershipService membershipService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var membership = await membershipService.GetMineAsync();
        if (membership is null)
        {
            return View(new MyMembershipViewModel(false, null, null, null, null, false));
        }

        var plan = await membershipService.GetPlanAsync(membership.MembershipPlanId);

        return View(new MyMembershipViewModel(
            true,
            plan?.Name ?? "Membership",
            membership.Status,
            membership.StartDate,
            membership.RenewalDate,
            membership.Status is "Active" or "PendingPayment" or "PastDue"));
    }
}
