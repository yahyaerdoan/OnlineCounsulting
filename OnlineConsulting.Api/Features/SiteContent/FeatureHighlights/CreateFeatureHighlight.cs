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
        app.MapPost("/api/site-content/feature-highlights", Handle)
            .WithTags("SiteContent/FeatureHighlights")
            .RequireAuthorization()
            .WithName("CreateFeatureHighlight")
            .WithDescription("Creates a feature highlight content block.");
    }

    private static async Task<IResult> Handle([FromBody] CreateFeatureHighlightCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
