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
        _ = app.MapPost("/site-content/gallery-categories", Handle)
            .WithTags("SiteContent/GalleryCategories")
            .RequireAuthorization()
            .WithName("CreateGalleryCategory")
            .WithDescription("Creates a gallery category tag.");
    }

    private static async Task<IResult> Handle([FromBody] CreateGalleryCategoryRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateGalleryCategoryRequest(string Name, string? Description = null)
{
    public CreateGalleryCategoryCommand ToCommand() => new(Name, Description);
}
