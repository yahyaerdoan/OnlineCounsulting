using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.ConfirmEmail;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class ConfirmEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/auth/confirm-email", Handle)
            .WithTags("Identity/Auth")
            .WithName("ConfirmEmail")
            .WithDescription("Confirms a user's email address using the token sent at registration.");
    }

    private static async Task<IResult> Handle([FromBody] ConfirmEmailCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
