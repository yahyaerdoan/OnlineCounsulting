using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.GetServiceAreaBySlug;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceAreas;

public class GetServiceAreaBySlug : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/site-content/service-areas/{slug}", Handle)
            .WithTags("SiteContent/ServiceAreas")
            .WithName("GetServiceAreaBySlug")
            .WithDescription("Returns a single service-area landing page by slug. Public - no login required.");
    }

    private static async Task<IResult> Handle(string slug, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetServiceAreaBySlugQuery(slug));
        return result.ToEnvelopedResult(httpContext);
    }
}
