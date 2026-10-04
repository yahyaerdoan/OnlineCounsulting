using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Services.Application.Features.Services.GetFeaturedServices;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Services;

public class GetFeaturedServices : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/services/featured", Handle)
            .WithTags("Services")
            .WithName("GetFeaturedServices")
            .WithDescription("Returns services marked as featured. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetFeaturedServicesQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
