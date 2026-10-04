using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.AssignRoleToUser;
using OnlineConsulting.Modules.Identity.Application.Features.Users.Contracts;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class AssignRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/users/{id:guid}/roles", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("AssignRoleToUser")
            .WithDescription("Assigns/unassigns roles for a user.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] AssignRoleToUserRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record AssignRoleToUserRequest(List<RoleAssignmentRequest> RoleAssignments)
{
    public AssignRoleToUserCommand ToCommand(Guid userId) => new(userId, RoleAssignments);
}
