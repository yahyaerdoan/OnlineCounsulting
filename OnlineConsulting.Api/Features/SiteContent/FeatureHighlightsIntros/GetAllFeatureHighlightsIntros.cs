using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.GetAllFeatureHighlightsIntros;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlightsIntros;

public class GetAllFeatureHighlightsIntros : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/site-content/feature-highlights-intro", Handle)
            .WithTags("SiteContent/FeatureHighlightsIntros")
            .WithName("GetAllFeatureHighlightsIntros")
            .WithDescription("Returns the tenant's feature highlights section intro. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllFeatureHighlightsIntrosQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
