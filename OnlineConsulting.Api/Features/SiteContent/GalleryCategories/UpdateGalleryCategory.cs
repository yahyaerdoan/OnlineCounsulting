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
        _ = app.MapPut("/site-content/gallery-categories/{id:guid}", Handle)
            .WithTags("SiteContent/GalleryCategories")
            .RequireAuthorization()
            .WithName("UpdateGalleryCategory")
            .WithDescription("Updates a gallery category tag.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateGalleryCategoryRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateGalleryCategoryRequest(string Name, string? Description = null)
{
    public UpdateGalleryCategoryCommand ToCommand(Guid id) => new(id, Name, Description);
}
