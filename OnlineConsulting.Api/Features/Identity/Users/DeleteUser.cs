using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.DeleteUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class DeleteUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/users/{id:guid}", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("DeleteUser")
            .WithDescription("Deletes a user.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteUserCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
