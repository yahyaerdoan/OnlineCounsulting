using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetUserRoles;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Users;

public class GetUserRoles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/users/{id:guid}/roles", Handle)
            .WithTags("Identity/Users")
            .RequireAuthorization()
            .WithName("GetUserRoles")
            .WithDescription("Returns every role and whether it's assigned to the given user.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetUserRolesQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
