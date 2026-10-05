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
        _ = app.MapPost("/auth/forgot-password", Handle)
            .WithTags("Identity/Auth")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("ForgotPassword")
            .WithDescription("Sends a password reset link if the email matches an account.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle([FromBody] ForgotPasswordRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record ForgotPasswordRequest(string Email)
{
    public ForgotPasswordCommand ToCommand() => new(Email);
}
