using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.Contracts;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.GetMembershipPlans;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.MembershipPlans;

public class GetMembershipPlans : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/membership-plans", Handle)
            .WithTags("Memberships/Plans")
            .WithName("GetMembershipPlans")
            .WithDescription("Returns the current tenant's membership plans, paginated. Public - used by the pricing page.")
            .ProducesEnveloped<Paginate<MembershipPlanResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null, bool includeArchived = false)
    {
        var result = await sender.Send(new GetMembershipPlansQuery(PageRequestFactory.Create(index, size), includeArchived));
        return result.ToEnvelopedResult(httpContext);
    }
}
