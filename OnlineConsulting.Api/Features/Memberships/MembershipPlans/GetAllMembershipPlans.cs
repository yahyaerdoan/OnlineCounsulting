using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.MembershipPlans.GetAllMembershipPlans;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.MembershipPlans;

public class GetAllMembershipPlans : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/membership-plans", Handle)
            .WithTags("Memberships/Plans")
            .WithName("GetAllMembershipPlans")
            .WithDescription("Returns the current tenant's membership plans, paginated. Public - used by the pricing page.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, int? index = null, int? size = null)
    {
        var result = await sender.Send(new GetAllMembershipPlansQuery(PageRequestFactory.Create(index, size)));
        return result.ToEnvelopedResult(httpContext);
    }
}
