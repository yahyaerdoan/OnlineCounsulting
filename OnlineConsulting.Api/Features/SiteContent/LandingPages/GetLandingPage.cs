using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingPages.GetLandingPage;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LandingPages;

public class GetLandingPage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/site-content/landing-page", Handle)
            .WithTags("SiteContent/LandingPage")
            .WithName("GetLandingPage")
            .WithDescription("Returns the platform landing copy. Public - no login required.")
            .ProducesEnveloped<LandingPageResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetLandingPageQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
