using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetRolePermissions;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class GetRolePermissions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/roles/{id:guid}/permissions", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("GetRolePermissions")
            .WithDescription("Returns the permission claims currently granted to a role.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetRolePermissionsQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
