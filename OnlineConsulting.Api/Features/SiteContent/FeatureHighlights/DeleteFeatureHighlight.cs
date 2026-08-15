using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.DeleteFeatureHighlight;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlights;

public class DeleteFeatureHighlight : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/site-content/feature-highlights/{id:guid}", Handle)
            .WithTags("SiteContent/FeatureHighlights")
            .RequireAuthorization()
            .WithName("DeleteFeatureHighlight")
            .WithDescription("Deletes a feature highlight content block.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteFeatureHighlightCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
