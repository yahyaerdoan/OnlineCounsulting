using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.DeleteHeroSlide;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.HeroSlides;

public class DeleteHeroSlide : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/hero-slides/{id:guid}", Handle)
            .WithTags("SiteContent/HeroSlides")
            .RequireAuthorization()
            .WithName("DeleteHeroSlide")
            .WithDescription("Deletes a homepage hero slide.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteHeroSlideCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
