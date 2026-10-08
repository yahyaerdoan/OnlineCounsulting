using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Contracts;
using ResultHandler.AspNetCore.Extensions;
using AuthRefresh = OnlineConsulting.Modules.Identity.Application.Features.Auth.RefreshToken;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/auth/refresh", Handle)
            .WithTags("Identity/Auth")
            .WithName("RefreshToken")
            .WithDescription("Exchanges an expired access token + valid refresh token for a new pair.")
            .ProducesEnveloped<AuthTokensResponse>();
    }

    private static async Task<IResult> Handle([FromBody] RefreshTokenRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record RefreshTokenRequest(string AccessToken, string RefreshToken)
{
    public AuthRefresh.RefreshTokenCommand ToCommand() => new(AccessToken, RefreshToken);
}
