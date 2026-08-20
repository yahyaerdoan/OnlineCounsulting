using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.DeleteGalleryCategory;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryCategories;

public class DeleteGalleryCategory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/gallery-categories/{id:guid}", Handle)
            .WithTags("SiteContent/GalleryCategories")
            .RequireAuthorization()
            .WithName("DeleteGalleryCategory")
            .WithDescription("Deletes a gallery category tag.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteGalleryCategoryCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
