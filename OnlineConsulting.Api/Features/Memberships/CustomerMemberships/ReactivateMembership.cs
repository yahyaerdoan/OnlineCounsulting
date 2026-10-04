using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.ReactivateMembership;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class ReactivateMembership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/memberships/reactivate", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("ReactivateMembership")
            .WithDescription("Undoes the current user's pending cancellation before the period ends - the membership renews again, no new charge.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new ReactivateMembershipCommand(user.Id))))
            .ToEnvelopedResult(httpContext);
}
