using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.DeviceTokens.RemoveDeviceToken;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.DeviceTokens;

public class RemoveDeviceToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/device-tokens/{token}", Handle)
            .WithTags("Identity/DeviceTokens")
            .RequireAuthorization()
            .WithName("RemoveDeviceToken")
            .WithDescription("Removes a device's push-notification token (call on logout / push opt-out).");
    }

    private static async Task<IResult> Handle(string token, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new RemoveDeviceTokenCommand(token));
        return result.ToEnvelopedResult(httpContext);
    }
}
