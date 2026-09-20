using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.PauseMembership;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class PauseMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/memberships/pause", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("PauseMembership")
            .WithDescription("Pauses the current user's active membership - billing stops until resumed.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());

        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new PauseMembershipCommand(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
