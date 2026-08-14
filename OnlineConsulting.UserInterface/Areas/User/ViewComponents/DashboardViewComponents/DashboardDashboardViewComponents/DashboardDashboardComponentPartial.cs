using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.BusinessLogic.Concretions.Services;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardDashboardViewComponents;

public class DashboardDashboardComponentPartial(IServiceManager serviceManager, ISender sender) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var totalOrders = 0;
        var pendingOrderCount = 0;
        var paidOrderCount = 0;
        var cancelledOrderCount = 0;
        decimal totalSpent = 0;

        var resultUser = await sender.Send(new GetCurrentUserQuery());
        if (!resultUser.IsSuccessful || resultUser.Data is null)
            return Content(string.Empty);

        var userId = resultUser.Data.Id;

        var resultTotalOrders = await serviceManager.OrderService.GetTotalOrderCountAsync(userId, false, true);
        if (resultTotalOrders.IsSuccessful)
            totalOrders = resultTotalOrders.Data;

        var resultPendingOrderCount = await serviceManager.OrderService.GetOrderCountByOrderStatusAsync(userId, OrderStatus.Pending, false, true);
        if (resultPendingOrderCount.IsSuccessful)
            pendingOrderCount = resultPendingOrderCount.Data;

        var resultPaidOrderCount = await serviceManager.OrderService.GetOrderCountByPaymentStatusAsync(userId, PaymentStatus.Paid, false, true);
        if (resultPaidOrderCount.IsSuccessful)
            paidOrderCount = resultPaidOrderCount.Data;

        var resultCancelledOrderCount = await serviceManager.OrderService.GetOrderCountByOrderStatusAsync(userId, OrderStatus.Cancelled, false, true);
        if (resultCancelledOrderCount.IsSuccessful)
            cancelledOrderCount = resultCancelledOrderCount.Data;

        var resultTotalSpent = await serviceManager.OrderService.GetTotalSpentByUserIdAsync(userId, false, true);

        if (resultTotalSpent.IsSuccessful)
            totalSpent = resultTotalSpent.Data;


        ViewBag.TotalOrders = totalOrders;
        ViewBag.PendingOrders = pendingOrderCount;
        ViewBag.PaidOrders = paidOrderCount;
        ViewBag.CancelledOrders = cancelledOrderCount;
        ViewBag.TotalSpent = totalSpent;

        return View();
    }
}
