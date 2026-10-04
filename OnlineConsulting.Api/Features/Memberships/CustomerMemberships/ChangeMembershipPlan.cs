using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Identity.Application.Features.Users.GetCurrentUser;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.ChangeMembershipPlan;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class ChangeMembershipPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/memberships/change-plan", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("ChangeMembershipPlan")
            .WithDescription("Upgrades or downgrades the current user's active membership to a different plan, prorated.");
    }

    private static async Task<IResult> Handle(Guid newMembershipPlanId, ISender sender, HttpContext httpContext)
        => (await sender.Send(new GetCurrentUserQuery())
                .BindAsync(user => sender.Send(new ChangeMembershipPlanCommand(user.Id, newMembershipPlanId))))
            .ToEnvelopedResult(httpContext);
}
