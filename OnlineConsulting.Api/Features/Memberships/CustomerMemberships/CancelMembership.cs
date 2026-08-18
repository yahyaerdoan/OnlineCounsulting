using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.CancelMembership;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class CancelMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/memberships/cancel", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("CancelMembership")
            .WithDescription("Cancels the current user's active membership immediately.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());
        if (!currentUser.IsSuccessful || currentUser.Data is null)
            return currentUser.ToEnvelopedResult(httpContext);

        var result = await sender.Send(new CancelMembershipCommand(currentUser.Data.Id));
        return result.ToEnvelopedResult(httpContext);
    }
}
