using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.DeviceTokens.RegisterDeviceToken;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

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
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(command with { UserId = currentUser.Data.Id });
        return result.ToEnvelopedResult(httpContext);
    }
}
