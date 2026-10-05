using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.Contracts;
using OnlineConsulting.Modules.Referrals.Application.Features.Referrals.ListReferrals;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Referrals;

public class ListReferrals : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/referrals/query", Handle)
            .WithTags("Referrals")
            .RequireAuthorization()
            .WithName("ListReferrals")
            .WithDescription("Returns all referrals (Admin only), paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body, with the referrer's and referred user's email and user name for the referrals on the page.")
            .ProducesEnveloped<Paginate<AdminReferralResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQueryRequest? dynamicQuery)
        => (await sender.Send(new ListReferralsQuery(query.ToPageRequest(), dynamicQuery?.ToDynamicQuery()))).ToEnvelopedResult(httpContext);
}
