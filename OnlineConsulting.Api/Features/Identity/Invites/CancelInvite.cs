using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Invites.CancelInvite;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Identity.Invites;

public class CancelInvite : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/invites/{id:guid}", Handle)
            .WithTags("Identity/Invites")
            .RequireAuthorization()
            .WithName("CancelInvite")
            .WithDescription("Cancels a pending invite.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new CancelInviteCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
