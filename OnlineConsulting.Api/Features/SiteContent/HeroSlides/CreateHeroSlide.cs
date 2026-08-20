using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.CreateHeroSlide;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.HeroSlides;

public class CreateHeroSlide : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/site-content/hero-slides", Handle)
            .WithTags("SiteContent/HeroSlides")
            .RequireAuthorization()
            .WithName("CreateHeroSlide")
            .WithDescription("Creates a homepage hero slide.");
    }

    private static async Task<IResult> Handle([FromBody] CreateHeroSlideCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
