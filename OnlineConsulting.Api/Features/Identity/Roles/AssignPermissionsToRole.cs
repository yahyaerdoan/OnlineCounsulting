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
        _ = app.MapPut("/api/roles/{id:guid}/permissions", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("AssignPermissionsToRole")
            .WithDescription("Replaces a role's permission claims with the given set.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] AssignPermissionsToRoleCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { RoleId = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
