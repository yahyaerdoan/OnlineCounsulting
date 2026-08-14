using Hateoas;
using Hateoas.AspNetCore;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class GetCurrentUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/me", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithDescription("Returns the currently authenticated user.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new GetCurrentUserQuery());
        return result
            .OnSuccess(user => user.Links = BuildLinks(httpContext, linkGenerator, user.Id, includeSelf: true))
            .ToEnvelopedResult(httpContext);
    }

    /// <summary>No "GetUserById" route exists, so "self" only applies to the caller's own record, not list entries.</summary>
    internal static Dictionary<string, Link> BuildLinks(HttpContext httpContext, LinkGenerator linkGenerator, Guid id, bool includeSelf)
    {
        var builder = httpContext.Links(linkGenerator);
        if (includeSelf)
        {
            builder.Add("self", "GetCurrentUser", "GET");
        }
        return builder
            .AddCustom("assign-roles", "AssignRoleToUser", "PUT", new { id })
            .AddCustom("delete", "DeleteUser", "DELETE", new { id })
            .Build();
    }
}
