using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.ListUsers;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class ListUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/users/query", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("ListUsers")
            .WithDescription("Returns users, paginated (?index=&size=), optionally narrowed to one role (?role=) and filtered/sorted via a DynamicQuery body. POST rather than HTTP QUERY, since Swagger can't document that verb.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery,
        [FromQuery] string? role = null)
    {
        var result = await sender.Send(new ListUsersQuery(query.ToPageRequest(), dynamicQuery, role));

        return result.ToEnvelopedResult(httpContext);
    }
}
