using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.ListFeatureHighlightsIntros;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlightsIntros;

public class ListFeatureHighlightsIntros : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/feature-highlights-intro/query", Handle)
            .WithTags("SiteContent/FeatureHighlightsIntros")
            .WithName("ListFeatureHighlightsIntros")
            .WithDescription("Returns feature highlights intro entries, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new ListFeatureHighlightsIntrosQuery(query.ToPageRequest(), dynamicQuery));
        return result.ToEnvelopedResult(httpContext);
    }
}
