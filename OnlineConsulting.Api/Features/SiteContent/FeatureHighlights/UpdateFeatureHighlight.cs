using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.UpdateFeatureHighlight;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlights;

public class UpdateFeatureHighlight : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/feature-highlights/{id:guid}", Handle)
            .WithTags("SiteContent/FeatureHighlights")
            .RequireAuthorization()
            .WithName("UpdateFeatureHighlight")
            .WithDescription("Updates a feature highlight content block.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateFeatureHighlightCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
