using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Memberships.Application.Features.CustomerMemberships.GetAllCustomerMembershipsPaged;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Memberships.CustomerMemberships;

public class GetAllCustomerMembershipsPaged : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/memberships/query", Handle)
            .WithTags("Memberships/CustomerMemberships")
            .RequireAuthorization()
            .WithName("GetAllCustomerMembershipsPaged")
            .WithDescription("Returns all customer memberships with member and plan details, paginated (?index=&size=), optionally narrowed by ?search= (member name/email, plan, status) and ?view= (Active, Ending, NeedsAttention, Paused, Cancelled) and sorted via a DynamicQuery body. Admin only.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery,
        [FromQuery] string? search = null, [FromQuery] string? view = null)
    {
        var result = await sender.Send(new GetAllCustomerMembershipsPagedQuery(query.ToPageRequest(), dynamicQuery, search, view));
        return result.ToEnvelopedResult(httpContext);
    }
}
