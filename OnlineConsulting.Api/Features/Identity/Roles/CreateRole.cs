using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Roles.CreateRole;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Roles;

public class CreateRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/roles", Handle)
            .WithTags("Identity/Roles")
            .RequireAuthorization()
            .WithName("CreateRole")
            .WithDescription("Creates a new role.")
            .ProducesEnveloped(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateRoleRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateRoleRequest(string Name, string? Description)
{
    public CreateRoleCommand ToCommand() => new(Name, Description);
}
