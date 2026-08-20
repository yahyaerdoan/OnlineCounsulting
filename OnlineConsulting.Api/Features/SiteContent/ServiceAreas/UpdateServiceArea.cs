using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.UpdateServiceArea;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceAreas;

public class UpdateServiceArea : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/service-areas/{id:guid}", Handle)
            .WithTags("SiteContent/ServiceAreas")
            .RequireAuthorization()
            .WithName("UpdateServiceArea")
            .WithDescription("Updates a service-area SEO landing page.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateServiceAreaCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
