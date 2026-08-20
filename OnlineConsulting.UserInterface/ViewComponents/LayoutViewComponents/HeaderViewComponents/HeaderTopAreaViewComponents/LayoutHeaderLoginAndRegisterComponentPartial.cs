using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Common;
using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.ViewComponents.LayoutViewComponents.HeaderViewComponents.HeaderTopAreaViewComponents;

public class LayoutHeaderLoginAndRegisterComponentPartial(IApiClient apiClient) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await apiClient.GetAsync<CurrentUserResponse>("/api/users/me");

        ViewBag.userFullName = userResult.ResultData is not null ? $"{userResult.ResultData.FirstName} {userResult.ResultData.LastName}" : null;

        return View();
    }
}
