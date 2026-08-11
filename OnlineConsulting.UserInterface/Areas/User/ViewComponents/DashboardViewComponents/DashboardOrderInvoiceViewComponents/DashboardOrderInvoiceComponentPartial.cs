using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardOrderInvoiceViewComponents;

public class DashboardOrderInvoiceComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Guid orderId)
    {
        var userResult = await serviceManager.SystemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful)
            return View(new ResultOrderDetailDto());

        var result = await serviceManager.OrderService.GetOrderDetailByIdAsync(orderId, userResult.Data.Id, false, true);

        ViewBag.CurrentUserFullName = $"{userResult.Data.FirstName} {userResult.Data.LastName}";

        return View(result.Data ?? new ResultOrderDetailDto());
    }
}
