using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.UpdateHeroSlide;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.HeroSlides;

public class UpdateHeroSlide : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/site-content/hero-slides/{id:guid}", Handle)
            .WithTags("SiteContent/HeroSlides")
            .RequireAuthorization()
            .WithName("UpdateHeroSlide")
            .WithDescription("Updates a homepage hero slide.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateHeroSlideRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateHeroSlideRequest(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdateHeroSlideCommand ToCommand(Guid id) => new(id, Title, Description, ImageUrl, DisplayOrder, Metadata);
}
