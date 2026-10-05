using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.Register;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/auth/register", Handle)
            .WithTags("Identity/Auth")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("Register")
            .WithDescription("Creates a new user account.")
            .ProducesEnveloped(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] RegisterRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record RegisterRequest(string FirstName, string LastName, string UserName, string Email, string Password)
{
    public RegisterCommand ToCommand() => new(FirstName, LastName, UserName, Email, Password);
}
