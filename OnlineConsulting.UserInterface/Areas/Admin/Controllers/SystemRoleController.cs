using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.SystemRoleDtos;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.CreateRole;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.DeleteRole;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllRoles;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetRoleById;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.UpdateRole;
using OnlineConsulting.UserInterface.NotificationServices.ToastrServices;

namespace OnlineConsulting.UserInterface.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/{controller}/{action}/{id?}")]
[Authorize(Policy = "RequireAdminOrSuperAdminPolicy")]

public class SystemRoleController(ISender sender, IToastNotification toastNotification) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await sender.Send(new GetAllRolesQuery());

        var roles = (result.Data ?? []).Select(r => new ResultSystemRoleDto { Id = r.Id, Name = r.Name, Description = r.Description }).ToList();

        return View(roles);
    }
    [HttpGet]
    public IActionResult Create() => View();
    [HttpPost]
    public async Task<IActionResult> Create(CreateSystemRoleDto createSystemRoleDto)
    {
        var result = await sender.Send(new CreateRoleCommand(createSystemRoleDto.Name, createSystemRoleDto.Description));
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var result = await sender.Send(new GetRoleByIdQuery(id));
        var role = result.Data is { } r ? new ResultSystemRoleDto { Id = r.Id, Name = r.Name, Description = r.Description } : null;
        return View(role);
    }
    [HttpPost]
    public async Task<IActionResult> Update(UpdateSystemRoleDto updateSystemRoleDto)
    {
        var result = await sender.Send(new UpdateRoleCommand(updateSystemRoleDto.Id, updateSystemRoleDto.Name, updateSystemRoleDto.Description));
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await sender.Send(new DeleteRoleCommand(id));
        NToastService.Show(toastNotification, result.Title, result.Status);

        return RedirectToAction("Index");
    }
}
