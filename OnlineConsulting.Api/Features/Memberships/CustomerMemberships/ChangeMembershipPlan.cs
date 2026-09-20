using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.ChangeMembershipPlan;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class ChangeMembershipPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/memberships/change-plan", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("ChangeMembershipPlan")
            .WithDescription("Upgrades or downgrades the current user's active membership to a different plan, prorated.");
    }

    private static async Task<IResult> Handle(Guid newMembershipPlanId, ISender sender, HttpContext httpContext)
    {
        var currentUser = await sender.Send(new GetCurrentUserQuery());

        if (!currentUser.IsSuccessful || currentUser.Data is null)
        {
            return currentUser.ToEnvelopedResult(httpContext);
        }

        var result = await sender.Send(new ChangeMembershipPlanCommand(currentUser.Data.Id, newMembershipPlanId));
        return result.ToEnvelopedResult(httpContext);
    }
}
