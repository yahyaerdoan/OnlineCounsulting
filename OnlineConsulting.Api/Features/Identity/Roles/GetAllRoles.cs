using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllRoles;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class GetAllRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/roles", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("GetAllRoles")
            .WithDescription("Returns all roles.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllRolesQuery());
        return result
            .OnSuccess(roles =>
            {
                foreach (var role in roles)
                {
                    role.Links = GetRoleById.BuildLinks(httpContext, linkGenerator, role.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
