using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardOrdersViewComponents;

public class DashboardOrdersComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await serviceManager.SystemUserService.GetCurrentUserAsync();
        if (!result.IsSuccessful || result.Data is null)
            return View(new List<ResultOrderDto>());

        var ordersResult = await serviceManager.OrderService.GetOrdersByUserIdAsync(result.Data.Id);
        if (!ordersResult.IsSuccessful || ordersResult.Data is null)
            return View(new List<ResultOrderDto>());

        var sortedOrders = ordersResult.Data
            .OrderByDescending(x => x.CreatedDate)
            .ToList();

        return View(sortedOrders);
    }
}
