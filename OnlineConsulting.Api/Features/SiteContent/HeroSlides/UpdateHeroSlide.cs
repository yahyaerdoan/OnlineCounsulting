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
        _ = app.MapPut("/api/site-content/hero-slides/{id:guid}", Handle)
            .WithTags("SiteContent/HeroSlides")
            .RequireAuthorization()
            .WithName("UpdateHeroSlide")
            .WithDescription("Updates a homepage hero slide.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateHeroSlideCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
