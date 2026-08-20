using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.GetFeaturedServices;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Functional;

namespace OnlineConsulting.Api.Features.Services;

public class GetFeaturedServices : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/services/featured", Handle)
            .WithTags("Services")
            .WithName("GetFeaturedServices")
            .WithDescription("Returns services marked as featured. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, LinkGenerator linkGenerator, HttpContext httpContext)
    {
        var result = await sender.Send(new GetFeaturedServicesQuery());
        return result
            .OnSuccess(items =>
            {
                foreach (var service in items)
                {
                    service.Links = GetServiceById.BuildLinks(httpContext, linkGenerator, service.Id);
                }
            })
            .ToEnvelopedResult(httpContext);
    }
}
