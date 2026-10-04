using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.GetMyPreviousMembership;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class GetMyPreviousMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/memberships/mine/previous", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("GetMyPreviousMembership")
            .WithDescription("Returns the current user's most recent ended membership with its plan, for the rejoin offer; 404 if none or already a member.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new GetMyPreviousMembershipQuery(user.Id))))
            .ToEnvelopedResult(httpContext);
}
