using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.UpdatePageBanner;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.PageBanners;

public class UpdatePageBanner : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/page-banners/{id:guid}", Handle)
            .WithTags("SiteContent/PageBanners")
            .RequireAuthorization()
            .WithName("UpdatePageBanner")
            .WithDescription("Updates a page header banner.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdatePageBannerCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
