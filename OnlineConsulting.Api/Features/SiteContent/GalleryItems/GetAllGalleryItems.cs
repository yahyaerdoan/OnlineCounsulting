using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.GetAllGalleryItems;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.GalleryItems;

public class GetAllGalleryItems : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/site-content/gallery-items", Handle)
            .WithTags("SiteContent/GalleryItems")
            .WithName("GetAllGalleryItems")
            .WithDescription("Returns the tenant's gallery items with their category tags. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllGalleryItemsQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
