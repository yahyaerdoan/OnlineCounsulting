using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.ResetPassword;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class ResetPassword : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/auth/reset-password", Handle)
            .WithTags("Identity/Auth")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("ResetPassword")
            .WithDescription("Sets a new password using the token emailed by ForgotPassword.");
    }

    private static async Task<IResult> Handle([FromBody] ResetPasswordRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record ResetPasswordRequest(Guid UserId, string Token, string NewPassword)
{
    public ResetPasswordCommand ToCommand() => new(UserId, Token, NewPassword);
}
