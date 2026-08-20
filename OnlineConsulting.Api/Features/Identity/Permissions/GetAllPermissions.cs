using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Permissions.GetAllPermissions;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Permissions;

public class GetAllPermissions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/permissions", Handle)
            .WithTags("Identity/Permissions")
            .RequireAuthorization()
            .WithName("GetAllPermissions")
            .WithDescription("Returns every permission defined in the system, grouped by module.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllPermissionsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
