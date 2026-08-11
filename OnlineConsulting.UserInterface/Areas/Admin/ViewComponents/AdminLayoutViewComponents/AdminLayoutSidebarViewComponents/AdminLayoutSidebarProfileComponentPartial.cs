using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;

namespace OnlineConsulting.UserInterface.Areas.Admin.ViewComponents.AdminLayoutViewComponents.AdminLayoutSidebarViewComponents;

public class AdminLayoutSidebarProfileComponentPartial(IServiceManager serviceManager) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await serviceManager.SystemUserService.GetCurrentUserAsync();
        if (!userResult.IsSuccessful || userResult.Data is null)
            return Content(string.Empty);

        var user = userResult.Data;
        var userRole = await serviceManager.SystemUserService.GetUserRolesAsync(user.Id);

        // Manually map roles to correct DTO
        user.Roles = [.. (userRole.Data ?? [])
            .Select(role => new ResultSystemRoleDto
            {
                Id = role.Id,
                Name = role.RoleName,
                IsExist = role.Exist,
            })
            .Where(r => r.IsExist)];

        return View(user);
    }
}
