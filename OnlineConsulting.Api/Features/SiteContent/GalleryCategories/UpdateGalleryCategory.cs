using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.UpdateGalleryCategory;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryCategories;

public class UpdateGalleryCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/site-content/gallery-categories/{id:guid}", Handle)
            .WithTags("SiteContent/GalleryCategories")
            .RequireAuthorization()
            .WithName("UpdateGalleryCategory")
            .WithDescription("Updates a gallery category tag.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateGalleryCategoryCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
