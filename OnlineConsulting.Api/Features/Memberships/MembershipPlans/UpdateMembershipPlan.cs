using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.UpdateMembershipPlan;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.MembershipPlans;

public class UpdateMembershipPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/membership-plans/{id:guid}", Handle)
            .WithTags("Memberships/Plans")
            .RequireAuthorization()
            .WithName("UpdateMembershipPlan")
            .WithDescription("Updates a membership plan's local fields (admin). Never changes the provider-side price.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateMembershipPlanCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
