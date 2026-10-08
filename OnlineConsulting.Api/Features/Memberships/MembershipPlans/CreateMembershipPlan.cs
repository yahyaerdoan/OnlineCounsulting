using Hateoas.AspNetCore;
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
        _ = app.MapPost("/membership-plans", Handle)
            .WithTags("Memberships/Plans")
            .RequireAuthorization()
            .WithName("CreateMembershipPlan")
            .WithCreatedLocation("GetMembershipPlanById")
            .WithDescription("Creates a membership plan (admin) and its provider-side product/price.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateMembershipPlanRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateMembershipPlanRequest(string Name, string BillingCycle, decimal Price, int IncludedVisitsPerYear, decimal DiscountPercent, decimal CreditAmount, string? Benefits, int? TrialDays = null)
{
    public CreateMembershipPlanCommand ToCommand() => new(Name, BillingCycle, Price, IncludedVisitsPerYear, DiscountPercent, CreditAmount, Benefits, TrialDays);
}
