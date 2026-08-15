using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.GetAllFeatureHighlights;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlights;

public class GetAllFeatureHighlights : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/site-content/feature-highlights", Handle)
            .WithTags("SiteContent/FeatureHighlights")
            .WithName("GetAllFeatureHighlights")
            .WithDescription("Returns the tenant's feature highlight content blocks. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllFeatureHighlightsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
