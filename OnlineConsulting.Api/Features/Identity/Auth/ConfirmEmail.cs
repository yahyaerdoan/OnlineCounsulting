using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.ConfirmEmail;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class ConfirmEmail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/auth/confirm-email", Handle)
            .WithTags("Identity/Auth")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("ConfirmEmail")
            .WithDescription("Confirms a user's email address using the token sent at registration.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle([FromBody] ConfirmEmailRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record ConfirmEmailRequest(Guid UserId, string Token)
{
    public ConfirmEmailCommand ToCommand() => new(UserId, Token);
}
