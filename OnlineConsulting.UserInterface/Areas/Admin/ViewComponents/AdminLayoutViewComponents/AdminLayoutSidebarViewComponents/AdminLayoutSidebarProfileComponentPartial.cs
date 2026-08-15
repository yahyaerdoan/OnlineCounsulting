using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserRoles;
using OnlineConsulting.UserInterface.Common;

namespace OnlineConsulting.UserInterface.Areas.Admin.ViewComponents.AdminLayoutViewComponents.AdminLayoutSidebarViewComponents;

public class AdminLayoutSidebarProfileComponentPartial(ISender sender) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userResult = await sender.Send(new GetCurrentUserQuery());
        if (!userResult.IsSuccessful || userResult.Data is null)
            return Content(string.Empty);

        var user = userResult.Data.ToResultUserDto();
        var userRole = await sender.Send(new GetUserRolesQuery(userResult.Data.Id));

        user.Roles = [.. (userRole.Data ?? [])
            .Select(role => new ResultSystemRoleDto
            {
                Id = role.RoleId,
                Name = role.RoleName,
                IsAssigned = role.IsAssigned,
            })
            .Where(r => r.IsAssigned)];

        return View(user);
    }
}
