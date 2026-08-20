using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.GetServiceBySlug;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Services;

public class GetServiceBySlug : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/services/by-slug/{slug}", Handle)
            .WithTags("Services")
            .WithName("GetServiceBySlug")
            .WithDescription("Returns a single service by its SEO slug. Public - no login required.");
    }

    private static async Task<IResult> Handle(string slug, ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new GetServiceBySlugQuery(slug));
        return result
            .OnSuccess(service => service.Links = GetServiceById.BuildLinks(httpContext, linkGenerator, service.Id))
            .ToEnvelopedResult(httpContext);
    }
}
