using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

public class DashboardOrderDetailComponentPartial(IUserOrderPageService userOrderPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Guid orderId) =>
        View(await userOrderPageService.GetMyOrderDetailAsync(orderId));
}
