using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.CreateServiceOffering;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceOfferings;

public class CreateServiceOffering : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/api/site-content/service-offerings", Handle)
            .WithTags("SiteContent/ServiceOfferings")
            .RequireAuthorization()
            .WithName("CreateServiceOffering")
            .WithDescription("Creates a card in the \"what we provide\" homepage section.");
    }

    private static async Task<IResult> Handle([FromBody] CreateServiceOfferingCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command);
        return result.ToEnvelopedResult(httpContext);
    }
}
