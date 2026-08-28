using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetAllUsers;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class GetAllUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // POST /query, not HTTP QUERY - Swagger can't document that verb.
        _ = app.MapPost("/api/users/query", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("GetAllUsers")
            .WithDescription("Returns users, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new GetAllUsersQuery(query.ToPageRequest(), dynamicQuery));
        return result
            .OnSuccess(page =>
            {
                foreach (var user in page.Items)
                {
                    user.Links = GetCurrentUser.BuildLinks(httpContext, linkGenerator, user.Id, includeSelf: false);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
