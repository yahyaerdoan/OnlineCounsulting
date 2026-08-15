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
        app.MapPut("/api/site-content/gallery-items/{id:guid}", Handle)
            .WithTags("SiteContent/GalleryItems")
            .RequireAuthorization()
            .WithName("UpdateGalleryItem")
            .WithDescription("Updates a gallery item and replaces its category tags.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateGalleryItemCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
