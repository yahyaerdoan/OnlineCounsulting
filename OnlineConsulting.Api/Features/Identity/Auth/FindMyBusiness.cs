using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Api.Configurations.Extensions;
using OnlineConsulting.Modules.Identity.Application.Features.Auth.FindMyBusiness;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class FindMyBusiness : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/auth/find-my-business", Handle)
            .WithTags("Identity/Auth")
            .RequireRateLimiting(ServiceRegistration.AuthRateLimiterPolicy)
            .WithName("FindMyBusiness")
            .WithDescription("Emails the sign-in links of every business the address has an account with. Always succeeds, so it never reveals whether the address is registered.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle([FromBody] FindMyBusinessRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record FindMyBusinessRequest(string Email)
{
    public FindMyBusinessCommand ToCommand() => new(Email);
}
