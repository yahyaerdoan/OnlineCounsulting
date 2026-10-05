using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.CreateInvite;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Auth;

public class CreateInvite : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/auth/invites", Handle)
            .WithTags("Identity/Auth")
            .RequireAuthorization()
            .WithName("CreateInvite")
            .WithDescription("Invites a new teammate into the caller's own tenant by email.")
            .ProducesEnveloped(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateInviteRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateInviteRequest(string Email, string? RoleName = null)
{
    public CreateInviteCommand ToCommand() => new(Email, RoleName);
}
