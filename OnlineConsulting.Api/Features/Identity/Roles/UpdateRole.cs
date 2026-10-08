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
        _ = app.MapPut("/roles/{id:guid}", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("UpdateRole")
            .WithDescription("Updates an existing role.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateRoleRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateRoleRequest(string Name, string? Description)
{
    public UpdateRoleCommand ToCommand(Guid id) => new(id, Name, Description);
}
