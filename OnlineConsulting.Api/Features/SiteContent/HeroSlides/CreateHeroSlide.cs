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
        _ = app.MapPost("/site-content/hero-slides", Handle)
            .WithTags("SiteContent/HeroSlides")
            .RequireAuthorization()
            .WithName("CreateHeroSlide")
            .WithDescription("Creates a homepage hero slide.");
    }

    private static async Task<IResult> Handle([FromBody] CreateHeroSlideRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateHeroSlideRequest(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public CreateHeroSlideCommand ToCommand() => new(Title, Description, ImageUrl, DisplayOrder, Metadata);
}
