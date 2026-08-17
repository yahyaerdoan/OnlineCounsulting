using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetAllPermissions;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class GetAllPermissions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/permissions", Handle)
            .WithTags("Identity/Roles")
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
