using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserPermissionOverrides;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class GetUserPermissionOverrides : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/users/{id:guid}/permission-overrides", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("GetUserPermissionOverrides")
            .WithDescription("Returns a user's role-granted permissions and which ones have been individually denied.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetUserPermissionOverridesQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
