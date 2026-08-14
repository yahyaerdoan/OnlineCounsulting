using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.ChangePassword;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class ChangePassword : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/users/me/password", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("ChangePassword")
            .WithDescription("Changes the current user's password.");
    }

    private static async Task<IResult> Handle([FromBody] ChangePasswordCommand command, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(command with { UserId = currentUser.Data.Id });
        return result.ToEnvelopedResult(httpContext);
    }
}
