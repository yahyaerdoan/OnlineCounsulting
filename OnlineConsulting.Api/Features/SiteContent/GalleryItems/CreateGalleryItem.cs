using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.CreateGalleryItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryItems;

public class CreateGalleryItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/gallery-items", Handle)
            .WithTags("SiteContent/GalleryItems")
            .RequireAuthorization()
            .WithName("CreateGalleryItem")
            .WithDescription("Creates a gallery item, tagged with one or more gallery categories.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateGalleryItemRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateGalleryItemRequest(string Description, List<Guid> CategoryIds, Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public CreateGalleryItemCommand ToCommand() => new(Description, CategoryIds, PhotoMediaAssetId, DisplayOrder, Metadata);
}
