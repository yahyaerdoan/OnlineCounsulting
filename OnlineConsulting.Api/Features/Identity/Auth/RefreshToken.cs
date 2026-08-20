using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using ResultHandler.AspNetCore.Extensions;
using AuthRefresh = OnlineConsulting.Modules.Identity.Application.Features.Auth.RefreshToken;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/auth/refresh", Handle)
            .WithTags("Identity/Auth")
            .WithName("RefreshToken")
            .WithDescription("Exchanges an expired access token + valid refresh token for a new pair.");
    }

    private static async Task<IResult> Handle([FromBody] AuthRefresh.RefreshTokenCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
