using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Common;
using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Order;

/// <summary>Same order data as the detail component; the invoice additionally prints the customer's full name.</summary>
public class DashboardOrderInvoiceComponentPartial(IUserOrderPageService userOrderPageService, IApiClient apiClient) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Guid orderId)
    {
        var userResult = await apiClient.GetAsync<CurrentUserResponse>("/api/users/me");
        if (userResult.IsSuccessful && userResult.ResultData is not null)
            ViewBag.CurrentUserFullName = $"{userResult.ResultData.FirstName} {userResult.ResultData.LastName}";

        return View(await userOrderPageService.GetMyOrderDetailAsync(orderId));
    }
}
