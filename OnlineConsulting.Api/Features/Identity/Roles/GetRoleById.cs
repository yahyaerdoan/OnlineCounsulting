using Hateoas;
using Hateoas.AspNetCore;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetRoleById;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class GetRoleById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/roles/{id:guid}", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("GetRoleById")
            .WithDescription("Returns a single role by id.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new GetRoleByIdQuery(id));
        return result
            .OnSuccess(role => role.Links = BuildLinks(httpContext, linkGenerator, role.Id))
            .ToEnvelopedResult(httpContext);
    }

    internal static Dictionary<string, Link> BuildLinks(HttpContext httpContext, LinkGenerator linkGenerator, Guid id)
        => httpContext.Links(linkGenerator)
            .Add("self", "GetRoleById", "GET", new { id })
            .Add("edit", "UpdateRole", "PUT", new { id })
            .AddCustom("delete", "DeleteRole", "DELETE", new { id })
            .Build();
}
