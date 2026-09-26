using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.DeviceTokens.RegisterDeviceToken;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.DeviceTokens;

public class RegisterDeviceToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/device-tokens", Handle)
            .WithTags("Identity/DeviceTokens")
            .RequireAuthorization()
            .WithName("RegisterDeviceToken")
            .WithDescription("Registers (or re-registers) the current user's mobile device push-notification token.");
    }

    private static async Task<IResult> Handle([FromBody] RegisterDeviceTokenCommand command, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(command with { UserId = user.Id })))
            .ToEnvelopedResult(httpContext);
}
