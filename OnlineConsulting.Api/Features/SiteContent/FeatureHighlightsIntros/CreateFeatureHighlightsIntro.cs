using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.CreateFeatureHighlightsIntro;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlightsIntros;

public class CreateFeatureHighlightsIntro : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/site-content/feature-highlights-intro", Handle)
            .WithTags("SiteContent/FeatureHighlightsIntros")
            .RequireAuthorization()
            .WithName("CreateFeatureHighlightsIntro")
            .WithDescription("Creates the feature highlights section intro (description + cover image).");
    }

    private static async Task<IResult> Handle([FromBody] CreateFeatureHighlightsIntroCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
