using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

/// <summary>The dashboard landing tile row - purely order statistics, so it belongs with the user Order feature.</summary>
public class DashboardDashboardComponentPartial(IUserOrderPageService userOrderPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var stats = await userOrderPageService.GetMyStatsAsync();

        ViewBag.TotalOrders = stats.TotalOrders;
        ViewBag.PendingOrders = stats.PendingOrders;
        ViewBag.PaidOrders = stats.PaidOrders;
        ViewBag.CancelledOrders = stats.CancelledOrders;
        ViewBag.TotalSpent = stats.TotalSpent;

        return View();
    }
}
