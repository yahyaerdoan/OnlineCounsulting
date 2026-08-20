using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.PageBanners.GetAllPageBanners;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.PageBanners;

public class GetAllPageBanners : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/site-content/page-banners", Handle)
            .WithTags("SiteContent/PageBanners")
            .WithName("GetAllPageBanners")
            .WithDescription("Returns the tenant's page header banners. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllPageBannersQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
