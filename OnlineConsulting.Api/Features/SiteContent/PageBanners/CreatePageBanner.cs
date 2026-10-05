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
        _ = app.MapPost("/site-content/page-banners", Handle)
            .WithTags("SiteContent/PageBanners")
            .RequireAuthorization()
            .WithName("CreatePageBanner")
            .WithDescription("Creates a page header banner.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreatePageBannerRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreatePageBannerRequest(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public CreatePageBannerCommand ToCommand() => new(Title, Description, ImageUrl, DisplayOrder, Metadata);
}
