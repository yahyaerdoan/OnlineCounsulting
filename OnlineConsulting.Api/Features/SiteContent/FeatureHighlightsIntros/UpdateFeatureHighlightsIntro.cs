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
        _ = app.MapPut("/site-content/feature-highlights-intro/{id:guid}", Handle)
            .WithTags("SiteContent/FeatureHighlightsIntros")
            .RequireAuthorization()
            .WithName("UpdateFeatureHighlightsIntro")
            .WithDescription("Updates the feature highlights section intro.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateFeatureHighlightsIntroRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateFeatureHighlightsIntroRequest(string Description, Guid? CoverMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdateFeatureHighlightsIntroCommand ToCommand(Guid id) => new(id, Description, CoverMediaAssetId, DisplayOrder, Metadata);
}
