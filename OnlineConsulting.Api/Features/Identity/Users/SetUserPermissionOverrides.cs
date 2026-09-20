using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.SetUserPermissionOverrides;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class SetUserPermissionOverrides : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/users/{id:guid}/permission-overrides", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("SetUserPermissionOverrides")
            .WithDescription("Replaces the set of permissions individually denied for a user, narrowing their role's default grant.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] SetUserPermissionOverridesCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { UserId = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
