using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.OrderDtos;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardOrderInvoiceViewComponents;

public class DashboardOrderInvoiceComponentPartial(IServiceManager serviceManager, ISender sender) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Guid orderId)
    {
        var userResult = await sender.Send(new GetCurrentUserQuery());
        if (!userResult.IsSuccessful)
            return View(new ResultOrderDetailDto());

        var result = await serviceManager.OrderService.GetOrderDetailByIdAsync(orderId, userResult.Data.Id, false, true);

        ViewBag.CurrentUserFullName = $"{userResult.Data.FirstName} {userResult.Data.LastName}";

        return View(result.Data ?? new ResultOrderDetailDto());
    }
}
