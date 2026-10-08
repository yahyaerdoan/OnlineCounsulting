using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.UpdateGalleryItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryItems;

public class UpdateGalleryItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/site-content/gallery-items/{id:guid}", Handle)
            .WithTags("SiteContent/GalleryItems")
            .RequireAuthorization()
            .WithName("UpdateGalleryItem")
            .WithDescription("Updates a gallery item and replaces its category tags.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateGalleryItemRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateGalleryItemRequest(string Description, List<Guid> CategoryIds, Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdateGalleryItemCommand ToCommand(Guid id) => new(id, Description, CategoryIds, PhotoMediaAssetId, DisplayOrder, Metadata);
}
