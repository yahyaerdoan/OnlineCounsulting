using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.GetAllHeroSlides;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.HeroSlides;

public class GetAllHeroSlides : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/site-content/hero-slides", Handle)
            .WithTags("SiteContent/HeroSlides")
            .WithName("GetAllHeroSlides")
            .WithDescription("Returns the tenant's homepage hero slides. Public - no login required.")
            .ProducesEnveloped<List<HeroSlideResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllHeroSlidesQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
