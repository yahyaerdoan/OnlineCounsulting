using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.DeleteServiceArea;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceAreas;

public class DeleteServiceArea : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/service-areas/{id:guid}", Handle)
            .WithTags("SiteContent/ServiceAreas")
            .RequireAuthorization()
            .WithName("DeleteServiceArea")
            .WithDescription("Deletes a service-area SEO landing page.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteServiceAreaCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
