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
        app.MapPost("/api/site-content/gallery-items", Handle)
            .WithTags("SiteContent/GalleryItems")
            .RequireAuthorization()
            .WithName("CreateGalleryItem")
            .WithDescription("Creates a gallery item, tagged with one or more gallery categories.");
    }

    private static async Task<IResult> Handle([FromBody] CreateGalleryItemCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
