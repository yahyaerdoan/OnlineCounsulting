using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.ListPromotions;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Promotions;

public class ListPromotions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/promotions/query", Handle)
            .WithTags("SiteContent/Promotions")
            .WithName("ListPromotions")
            .WithDescription("Returns promotions, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.")
            .ProducesEnveloped<Paginate<PromotionResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQueryRequest? dynamicQuery)
    {
        var result = await sender.Send(new ListPromotionsQuery(query.ToPageRequest(), dynamicQuery?.ToDynamicQuery()));
        return result.ToEnvelopedResult(httpContext);
    }
}
