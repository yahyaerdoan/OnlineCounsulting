using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetAllUsers;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class GetAllUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/users", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("GetAllUsers")
            .WithDescription("Returns all users.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllUsersQuery());
        return result
            .OnSuccess(users =>
            {
                foreach (var user in users)
                {
                    user.Links = GetCurrentUser.BuildLinks(httpContext, linkGenerator, user.Id, includeSelf: false);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
