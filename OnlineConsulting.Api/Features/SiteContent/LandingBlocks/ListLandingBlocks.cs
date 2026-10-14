using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.ListLandingBlocks;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LandingBlocks;

public class ListLandingBlocks : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/landing-blocks/query", Handle)
            .WithTags("SiteContent/LandingBlocks")
            .WithName("ListLandingBlocks")
            .WithDescription("Returns landing blocks, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.")
            .ProducesEnveloped<Paginate<LandingBlockResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQueryRequest? dynamicQuery)
    {
        var result = await sender.Send(new ListLandingBlocksQuery(query.ToPageRequest(), dynamicQuery?.ToDynamicQuery()));
        return result.ToEnvelopedResult(httpContext);
    }
}
