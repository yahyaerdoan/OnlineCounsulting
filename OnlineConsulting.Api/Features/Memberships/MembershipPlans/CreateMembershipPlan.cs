using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.CreateMembershipPlan;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.MembershipPlans;

public class CreateMembershipPlan : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/membership-plans", Handle)
            .WithTags("Memberships/Plans")
            .RequireAuthorization()
            .WithName("CreateMembershipPlan")
            .WithDescription("Creates a membership plan (admin) and its provider-side product/price.");
    }

    private static async Task<IResult> Handle([FromBody] CreateMembershipPlanCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
