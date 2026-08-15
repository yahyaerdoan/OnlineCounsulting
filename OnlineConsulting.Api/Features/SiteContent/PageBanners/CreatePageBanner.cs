using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.CreatePageBanner;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.PageBanners;

public class CreatePageBanner : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/site-content/page-banners", Handle)
            .WithTags("SiteContent/PageBanners")
            .RequireAuthorization()
            .WithName("CreatePageBanner")
            .WithDescription("Creates a page header banner.");
    }

    private static async Task<IResult> Handle([FromBody] CreatePageBannerCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
