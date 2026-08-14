using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllRoles;
using OnlineConsulting.Modules.Identity.Application.Features.Users.AssignRoleToUser;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Users.DeleteUser;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetAllUsers;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserRoles;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class SystemUserController(ISender sender, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var usersResult = await sender.Send(new GetAllUsersQuery());
        var rolesResult = await sender.Send(new GetAllRolesQuery());
        var rolesByName = (rolesResult.Data ?? []).ToDictionary(r => r.Name);

        var users = (usersResult.Data ?? [])
            .Select(u => new ResultUserDto
            {
                Id = u.Id,
                TenantId = u.TenantId,
                Username = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                ImageUrl = u.ImageUrl ?? string.Empty,
                Roles = [.. u.Roles.Select(roleName => rolesByName.TryGetValue(roleName, out var role)
                    ? new ResultSystemRoleDto { Id = role.Id, Name = role.Name, Description = role.Description }
                    : new ResultSystemRoleDto { Name = roleName })],
            })
            .ToList();

        return View(users);
    }
    [HttpGet]
    public async Task<IActionResult> AssingARoleToUser(Guid id)
    {
        ViewBag.UserId = id;
        var result = await sender.Send(new GetUserRolesQuery(id));

        var roleAssignments = (result.Data ?? [])
            .Select(r => new AssingARoleToUserDto { RoleId = r.RoleId, RoleName = r.RoleName, IsAssigned = r.IsAssigned })
            .ToList();

        return View(roleAssignments);
    }
    [HttpPost]
    public async Task<IActionResult> AssingARoleToUser(Guid id, List<AssingARoleToUserDto> assingARoleToUserDtos)
    {
        var assignments = assingARoleToUserDtos.Select(d => new RoleAssignmentRequest(d.RoleId, d.RoleName, d.IsAssigned)).ToList();
        var result = await sender.Send(new AssignRoleToUserCommand(id, assignments));
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await sender.Send(new DeleteUserCommand(id));
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
