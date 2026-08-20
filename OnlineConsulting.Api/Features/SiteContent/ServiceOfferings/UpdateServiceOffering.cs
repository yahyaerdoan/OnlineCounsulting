using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.UpdateServiceOffering;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceOfferings;

public class UpdateServiceOffering : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/api/site-content/service-offerings/{id:guid}", Handle)
            .WithTags("SiteContent/ServiceOfferings")
            .RequireAuthorization()
            .WithName("UpdateServiceOffering")
            .WithDescription("Updates a service offering card.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateServiceOfferingCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
