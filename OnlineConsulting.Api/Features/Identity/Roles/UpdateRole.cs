using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.UpdateRole;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class UpdateRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/roles/{id:guid}", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("UpdateRole")
            .WithDescription("Updates an existing role.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateRoleCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
