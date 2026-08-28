using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllRolePermissions;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class GetAllRolePermissions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/roles/permissions", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("GetAllRolePermissions")
            .WithDescription("Returns every role's assigned permissions, for the permission matrix.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllRolePermissionsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
