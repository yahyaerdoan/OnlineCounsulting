using Core.PersistenceLayer.Dynamics.Dynamic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.ListHeroSlides;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.HeroSlides;

public class ListHeroSlides : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/site-content/hero-slides/query", Handle)
            .WithTags("SiteContent/HeroSlides")
            .WithName("ListHeroSlides")
            .WithDescription("Returns hero slides, paginated (?index=&size=), optionally filtered/sorted via a DynamicQuery body.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext, [AsParameters] ListQueryParameters query, [FromBody] DynamicQuery? dynamicQuery)
    {
        var result = await sender.Send(new ListHeroSlidesQuery(query.ToPageRequest(), dynamicQuery));
        return result.ToEnvelopedResult(httpContext);
    }
}
