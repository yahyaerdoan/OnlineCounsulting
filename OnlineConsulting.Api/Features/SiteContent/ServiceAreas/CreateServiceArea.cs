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
        _ = app.MapPost("/site-content/service-areas", Handle)
            .WithTags("SiteContent/ServiceAreas")
            .RequireAuthorization()
            .WithName("CreateServiceArea")
            .WithDescription("Creates a service-area SEO landing page.")
            .ProducesEnveloped<Guid>(StatusCodes.Status201Created);
    }

    private static async Task<IResult> Handle([FromBody] CreateServiceAreaRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record CreateServiceAreaRequest(string Name, string State, string? IntroText, int DisplayOrder = 0)
{
    public CreateServiceAreaCommand ToCommand() => new(Name, State, IntroText, DisplayOrder);
}
