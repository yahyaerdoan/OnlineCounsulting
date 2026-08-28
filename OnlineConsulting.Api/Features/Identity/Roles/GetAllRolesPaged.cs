using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllRoles;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class GetAllRolesPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/roles/query", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("GetAllRolesPaged")
            .WithDescription("Returns roles, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new GetAllRolesPagedQuery(query.ToPageRequest(), dynamicQuery));
        return result
            .OnSuccess(page =>
            {
                foreach (var role in page.Items)
                {
                    role.Links = GetRoleById.BuildLinks(httpContext, linkGenerator, role.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
