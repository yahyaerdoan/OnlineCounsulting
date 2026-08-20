using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.SocialLinks.GetAllSocialLinks;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.SocialLinks;

public class GetAllSocialLinks : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/site-content/social-links", Handle)
            .WithTags("SiteContent/SocialLinks")
            .WithName("GetAllSocialLinks")
            .WithDescription("Returns the tenant's site-wide social links (header/footer). Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllSocialLinksQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
