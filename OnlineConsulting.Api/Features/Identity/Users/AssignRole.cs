using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.AssignRoleToUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class AssignRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/users/{id:guid}/roles", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("AssignRoleToUser")
            .WithDescription("Assigns/unassigns roles for a user.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] AssignRoleToUserCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { UserId = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
