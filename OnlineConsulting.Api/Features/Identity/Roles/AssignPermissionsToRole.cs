using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.AssignPermissionsToRole;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class AssignPermissionsToRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/roles/{id:guid}/permissions", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("AssignPermissionsToRole")
            .WithDescription("Replaces a role's permission claims with the given set.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] AssignPermissionsToRoleRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record AssignPermissionsToRoleRequest(List<string> Permissions)
{
    public AssignPermissionsToRoleCommand ToCommand(Guid roleId) => new(roleId, Permissions);
}
