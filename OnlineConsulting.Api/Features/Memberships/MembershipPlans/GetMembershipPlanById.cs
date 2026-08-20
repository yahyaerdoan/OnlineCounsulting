using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.GetMembershipPlanById;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.MembershipPlans;

public class GetMembershipPlanById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/membership-plans/{id:guid}", Handle)
            .WithTags("Memberships/Plans")
            .WithName("GetMembershipPlanById")
            .WithDescription("Returns a single membership plan by id. Public - used by the pricing page.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetMembershipPlanByIdQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
