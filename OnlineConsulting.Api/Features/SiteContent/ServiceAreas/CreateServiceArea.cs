using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.CreateServiceArea;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceAreas;

public class CreateServiceArea : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/site-content/service-areas", Handle)
            .WithTags("SiteContent/ServiceAreas")
            .RequireAuthorization()
            .WithName("CreateServiceArea")
            .WithDescription("Creates a service-area SEO landing page.");
    }

    private static async Task<IResult> Handle([FromBody] CreateServiceAreaCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
