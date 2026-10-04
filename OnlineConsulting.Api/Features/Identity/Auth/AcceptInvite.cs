using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.AcceptInvite;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class AcceptInvite : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/auth/invites/accept", Handle)
            .WithTags("Identity/Auth")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("AcceptInvite")
            .WithDescription("Accepts a teammate invite and creates the invited person's account.");
    }

    private static async Task<IResult> Handle([FromBody] AcceptInviteRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record AcceptInviteRequest(string Token, string FirstName, string LastName, string Password, string? PhoneNumber = null)
{
    public AcceptInviteCommand ToCommand() => new(Token, FirstName, LastName, Password, PhoneNumber);
}
