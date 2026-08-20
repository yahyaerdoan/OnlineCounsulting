using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.DeleteGalleryItem;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryItems;

public class DeleteGalleryItem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/gallery-items/{id:guid}", Handle)
            .WithTags("SiteContent/GalleryItems")
            .RequireAuthorization()
            .WithName("DeleteGalleryItem")
            .WithDescription("Deletes a gallery item and its category tags.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteGalleryItemCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
