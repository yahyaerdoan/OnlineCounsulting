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
        _ = app.MapGet("/api/users/me", Handle)
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

    /// <summary>includeSelf is true only for GetCurrentUser's own response; GetAllUsers list rows skip "self" since browsing another user isn't the caller viewing themselves.</summary>
    internal static Dictionary<string, Link> BuildLinks(HttpContext httpContext, LinkGenerator linkGenerator, Guid id, bool includeSelf)
    {
        var builder = httpContext.Links(linkGenerator);
        if (includeSelf)
        {
            _ = builder.Add("self", "GetCurrentUser", HttpMethods.Get);
        }

        return builder
            .AddCustom("assign-roles", "AssignRoleToUser", HttpMethods.Put, new { id })
            .AddCustom("delete", "DeleteUser", HttpMethods.Delete, new { id })
            .Build();
    }
}
