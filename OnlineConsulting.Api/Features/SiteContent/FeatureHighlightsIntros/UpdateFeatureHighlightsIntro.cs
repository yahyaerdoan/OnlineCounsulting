using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.UpdateFeatureHighlightsIntro;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlightsIntros;

public class UpdateFeatureHighlightsIntro : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/feature-highlights-intro/{id:guid}", Handle)
            .WithTags("SiteContent/FeatureHighlightsIntros")
            .RequireAuthorization()
            .WithName("UpdateFeatureHighlightsIntro")
            .WithDescription("Updates the feature highlights section intro.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateFeatureHighlightsIntroCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
