using Core.PersistenceLayer.Dynamics.Dynamic;
using Core.PersistenceLayer.Pagings.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.ListFooterInfos;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FooterInfos;

public class ListFooterInfos : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/footer-info/query", Handle)
            .WithTags("SiteContent/FooterInfos")
            .WithName("ListFooterInfos")
            .WithDescription("Returns footer info entries, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.")
            .ProducesEnveloped<Paginate<FooterInfoResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQueryRequest? dynamicQuery)
    {
        var result = await sender.Send(new ListFooterInfosQuery(query.ToPageRequest(), dynamicQuery?.ToDynamicQuery()));
        return result.ToEnvelopedResult(httpContext);
    }
}
