using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.DeleteFeatureHighlightsIntro;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlightsIntros;

public class DeleteFeatureHighlightsIntro : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/feature-highlights-intro/{id:guid}", Handle)
            .WithTags("SiteContent/FeatureHighlightsIntros")
            .RequireAuthorization()
            .WithName("DeleteFeatureHighlightsIntro")
            .WithDescription("Deletes the feature highlights section intro.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteFeatureHighlightsIntroCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
