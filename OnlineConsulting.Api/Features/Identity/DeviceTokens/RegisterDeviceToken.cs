using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.DeviceTokens.RegisterDeviceToken;
using OnlineConsulting.SharedKernel.CurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.DeviceTokens;

public class RegisterDeviceToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/device-tokens", Handle)
            .WithTags("Identity/DeviceTokens")
            .RequireAuthorization()
            .WithName("RegisterDeviceToken")
            .WithDescription("Registers (or re-registers) the current user's mobile device push-notification token.");
    }

    private static async Task<IResult> Handle(ICurrentUserAccessor currentUser, [FromBody] RegisterDeviceTokenRequest request, ISender sender, HttpContext httpContext)
        => (await sender.Send(request.ToCommand(currentUser.RequiredId())))
            .ToEnvelopedResult(httpContext);
}

public record RegisterDeviceTokenRequest(string Token, string Platform)
{
    public RegisterDeviceTokenCommand ToCommand(Guid userId) => new(userId, Token, Platform);
}
