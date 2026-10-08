using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.Contracts;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.GetRoleById;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class GetRoleById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/roles/{id:guid}", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("GetRoleById")
            .WithDescription("Returns a single role by id.")
            .ProducesEnveloped<RoleResponse>();
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetRoleByIdQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
