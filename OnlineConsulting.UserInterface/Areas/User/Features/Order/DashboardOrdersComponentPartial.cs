using Microsoft.AspNetCore.Mvc;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

public class DashboardOrdersComponentPartial(IUserOrderPageService userOrderPageService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() =>
        View(await userOrderPageService.GetMyOrdersAsync());
}
