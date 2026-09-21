using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.UserInterface.Common;
using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.ViewComponents.AdminLayoutViewComponents.AdminLayoutSidebarViewComponents;

public class AdminLayoutSidebarProfileComponentPartial(IApiClient apiClient) : ViewComponent
{
    /// <summary>Renders nothing (not an error state) if the current-user call fails, so a stale/expired session degrades the sidebar quietly instead of breaking the layout.</summary>
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await apiClient.GetAsync<CurrentUserResponse>("/api/users/me");
        if (!userResult.IsSuccessful || userResult.ResultData is null)
        {
            return Content(string.Empty);
        }

        var user = userResult.ResultData.ToUserSummaryViewModel();
        var roleResult = await apiClient.GetAsync<List<RoleAssignmentResponse>>($"/api/users/{userResult.ResultData.Id}/roles");

        user.Roles = [.. (roleResult.ResultData ?? [])
            .Where(role => role.IsAssigned)
            .Select(role => new RoleSummaryViewModel(role.RoleId, role.RoleName, role.IsAssigned))];

        return View(user);
    }
}
