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
        _ = app.MapPut("/site-content/service-areas/{id:guid}", Handle)
            .WithTags("SiteContent/ServiceAreas")
            .RequireAuthorization()
            .WithName("UpdateServiceArea")
            .WithDescription("Updates a service-area SEO landing page.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateServiceAreaRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateServiceAreaRequest(string Name, string State, string? IntroText, int DisplayOrder = 0)
{
    public UpdateServiceAreaCommand ToCommand(Guid id) => new(id, Name, State, IntroText, DisplayOrder);
}
