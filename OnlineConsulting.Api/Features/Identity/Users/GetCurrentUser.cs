using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class GetCurrentUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/users/me", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithDescription("Returns the currently authenticated user.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetCurrentUserQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
