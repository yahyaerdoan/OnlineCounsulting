using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.CreateGalleryCategory;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryCategories;

public class CreateGalleryCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/site-content/gallery-categories", Handle)
            .WithTags("SiteContent/GalleryCategories")
            .RequireAuthorization()
            .WithName("CreateGalleryCategory")
            .WithDescription("Creates a gallery category tag.");
    }

    private static async Task<IResult> Handle([FromBody] CreateGalleryCategoryCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
