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
        _ = app.MapPut("/site-content/feature-highlights/{id:guid}", Handle)
            .WithTags("SiteContent/FeatureHighlights")
            .RequireAuthorization()
            .WithName("UpdateFeatureHighlight")
            .WithDescription("Updates a feature highlight content block.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateFeatureHighlightRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateFeatureHighlightRequest(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdateFeatureHighlightCommand ToCommand(Guid id) => new(id, Title, Description, ImageUrl, DisplayOrder, Metadata);
}
