using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.AboutUss.GetAllAboutUss;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.AboutUss;

public class GetAllAboutUss : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/site-content/about-us", Handle)
            .WithTags("SiteContent/AboutUs")
            .WithName("GetAllAboutUss")
            .WithDescription("Returns the tenant's About Us content blocks. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllAboutUssQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
