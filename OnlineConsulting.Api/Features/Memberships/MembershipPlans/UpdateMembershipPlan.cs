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
        _ = app.MapPut("/membership-plans/{id:guid}", Handle)
            .WithTags("Memberships/Plans")
            .RequireAuthorization()
            .WithName("UpdateMembershipPlan")
            .WithDescription("Updates a membership plan's local fields (admin). Never changes the provider-side price.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateMembershipPlanRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateMembershipPlanRequest(string Name, int IncludedVisitsPerYear, decimal DiscountPercent, decimal CreditAmount, string? Benefits)
{
    public UpdateMembershipPlanCommand ToCommand(Guid id) => new(id, Name, IncludedVisitsPerYear, DiscountPercent, CreditAmount, Benefits);
}
