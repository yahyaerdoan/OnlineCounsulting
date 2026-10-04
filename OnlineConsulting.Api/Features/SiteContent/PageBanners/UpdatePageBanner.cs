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
        _ = app.MapPut("/site-content/page-banners/{id:guid}", Handle)
            .WithTags("SiteContent/PageBanners")
            .RequireAuthorization()
            .WithName("UpdatePageBanner")
            .WithDescription("Updates a page header banner.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdatePageBannerRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdatePageBannerRequest(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdatePageBannerCommand ToCommand(Guid id) => new(id, Title, Description, ImageUrl, DisplayOrder, Metadata);
}
