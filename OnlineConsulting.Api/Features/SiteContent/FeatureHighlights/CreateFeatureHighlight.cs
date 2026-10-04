using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.CreateFeatureHighlight;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.FeatureHighlights;

public class CreateFeatureHighlight : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/feature-highlights", Handle)
            .WithTags("SiteContent/FeatureHighlights")
            .RequireAuthorization()
            .WithName("CreateFeatureHighlight")
            .WithDescription("Creates a feature highlight content block.");
    }

    private static async Task<IResult> Handle([FromBody] CreateFeatureHighlightRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateFeatureHighlightRequest(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public CreateFeatureHighlightCommand ToCommand() => new(Title, Description, ImageUrl, DisplayOrder, Metadata);
}
