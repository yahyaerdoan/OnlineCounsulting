using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.ListRoles;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class ListRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/roles/query", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("ListRoles")
            .WithDescription("Returns roles, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new ListRolesQuery(query.ToPageRequest(), dynamicQuery));
        return result.ToEnvelopedResult(httpContext);
    }
}
