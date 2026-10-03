using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.AssignPermissionsToRole;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.DeleteRole;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetRoleById;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetRolePermissions;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.UpdateRole;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public sealed class RoleLinks : LinkProvider<RoleResponse>
{
    protected override void AddLinks(RoleResponse resource, HateoasLinkBuilder links)
    {
        var id = new { id = resource.Id };
        var user = links.User;

        _ = links
            .AddIf(user.CanSend<GetRoleByIdQuery>(), LinkRelations.Self, "GetRoleById", HttpMethods.Get, id)
            .AddIf(user.CanSend<UpdateRoleCommand>(), LinkRelations.Edit, "UpdateRole", HttpMethods.Put, id)
            .AddCustomIf(user.CanSend<DeleteRoleCommand>(), Rels.Delete, "DeleteRole", HttpMethods.Delete, id)
            .AddCustomIf(user.CanSend<GetRolePermissionsQuery>(), Rels.Permissions, "GetRolePermissions", HttpMethods.Get, id)
            .AddCustomIf(user.CanSend<AssignPermissionsToRoleCommand>(), Rels.AssignPermissions, "AssignPermissionsToRole", HttpMethods.Put, id);
    }
}
