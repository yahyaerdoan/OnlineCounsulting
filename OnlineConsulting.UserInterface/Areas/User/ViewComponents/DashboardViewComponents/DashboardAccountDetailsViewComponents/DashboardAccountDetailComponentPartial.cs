using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Areas.User.Features.Dashboard;
using OnlineConsulting.UserInterface.Common;
using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.ViewComponents.DashboardViewComponents.DashboardAccountDetailsViewComponents;

public class DashboardAccountDetailComponentPartial(IApiClient apiClient) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await apiClient.GetAsync<CurrentUserResponse>("/api/users/me");
        if (!userResult.IsSuccessful || userResult.ResultData is null)
        {
            return Content(string.Empty);
        }

        var viewModel = new UserAccountViewModel
        {
            User = userResult.ResultData.ToUserSummaryViewModel()
        };

        return View(viewModel);
    }
}
