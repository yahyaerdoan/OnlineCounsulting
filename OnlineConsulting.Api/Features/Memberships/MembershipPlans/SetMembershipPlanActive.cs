using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.SetMembershipPlanActive;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.MembershipPlans;

public class SetMembershipPlanActive : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/membership-plans/{id:guid}/active", Handle)
            .WithTags("Memberships/Plans")
            .RequireAuthorization()
            .WithName("SetMembershipPlanActive")
            .WithDescription("Archives (IsActive=false) or restores (IsActive=true) a membership plan (admin). Archiving hides it from the public catalog and blocks new subscriptions without affecting existing subscribers.");
    }

    private static async Task<IResult> Handle(Guid id, bool isActive, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new SetMembershipPlanActiveCommand(id, isActive));
        return result.ToEnvelopedResult(httpContext);
    }
}
