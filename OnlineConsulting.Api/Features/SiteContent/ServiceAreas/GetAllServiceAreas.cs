using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.GetAllServiceAreas;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceAreas;

public class GetAllServiceAreas : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/site-content/service-areas", Handle)
            .WithTags("SiteContent/ServiceAreas")
            .WithName("GetAllServiceAreas")
            .WithDescription("Returns the tenant's service-area landing pages. Public - no login required.");
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllServiceAreasQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
