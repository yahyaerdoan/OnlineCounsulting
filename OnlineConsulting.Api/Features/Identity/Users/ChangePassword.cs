using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.ChangePassword;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class ChangePassword : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/users/me/password", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("ChangePassword")
            .WithDescription("Changes the current user's password.");
    }

    private static async Task<IResult> Handle([FromBody] ChangePasswordCommand command, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(command with { UserId = user.Id })))
            .ToEnvelopedResult(httpContext);
}
