using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.ForgotPassword;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class ForgotPassword : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/auth/forgot-password", Handle)
            .WithTags("Identity/Auth")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("ForgotPassword")
            .WithDescription("Sends a password reset link if the email matches an account.");
    }

    private static async Task<IResult> Handle([FromBody] ForgotPasswordCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
