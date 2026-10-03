using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Identity.Application.Features.Users.AssignRoleToUser;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Users.DeleteUser;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserById;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserPermissionOverrides;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserRoles;
using OnlineConsulting.Modules.Identity.Application.Features.Users.UpdateUser;

namespace OnlineConsulting.Api.Features.Identity.Users;

/// <summary>The caller's own account carries the self-service routes; another user carries the staff routes the caller is allowed to use (never delete yourself).</summary>
public sealed class UserLinks : LinkProvider<UserResponse>
{
    protected override void AddLinks(UserResponse resource, HateoasLinkBuilder links)
    {
        var user = links.User;

        if (user.IsUser(resource.Id))
        {
            _ = links
                .Self("GetCurrentUser")
                .AddCustom(Rels.ChangePassword, "ChangePassword", HttpMethods.Put)
                .AddCustom(Rels.UpdateImage, "UpdateUserImage", HttpMethods.Post);
            return;
        }

        var id = new { id = resource.Id };
        _ = links
            .AddIf(user.CanSend<GetUserByIdQuery>(), LinkRelations.Self, "GetUserById", HttpMethods.Get, id)
            .AddIf(user.CanSend<UpdateUserCommand>(), LinkRelations.Edit, "UpdateUser", HttpMethods.Put, id)
            .AddCustomIf(user.CanSend<GetUserRolesQuery>(), Rels.Roles, "GetUserRoles", HttpMethods.Get, id)
            .AddCustomIf(user.CanSend<AssignRoleToUserCommand>(), Rels.AssignRoles, "AssignRoleToUser", HttpMethods.Put, id)
            .AddCustomIf(user.CanSend<GetUserPermissionOverridesQuery>(), Rels.PermissionOverrides, "GetUserPermissionOverrides", HttpMethods.Get, id)
            .AddCustomIf(user.CanSend<DeleteUserCommand>(), Rels.Delete, "DeleteUser", HttpMethods.Delete, id);
    }
}
