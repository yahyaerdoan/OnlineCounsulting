using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.UpdateUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class UpdateUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/users/{id:guid}", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("UpdateUser")
            .WithDescription("Updates an existing user's profile and active status.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateUserCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
