using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.GetAllGalleryCategories;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryCategories;

public class GetAllGalleryCategories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/site-content/gallery-categories", Handle)
            .WithTags("SiteContent/GalleryCategories")
            .WithName("GetAllGalleryCategories")
            .WithDescription("Returns the tenant's gallery category tags.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllGalleryCategoriesQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
