using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.DeletePageBanner;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.PageBanners;

public class DeletePageBanner : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/site-content/page-banners/{id:guid}", Handle)
            .WithTags("SiteContent/PageBanners")
            .RequireAuthorization()
            .WithName("DeletePageBanner")
            .WithDescription("Deletes a page header banner.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeletePageBannerCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
