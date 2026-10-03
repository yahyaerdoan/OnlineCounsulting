using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllRoles;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class GetAllRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/roles", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("GetAllRoles")
            .WithDescription("Returns all roles.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllRolesQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
