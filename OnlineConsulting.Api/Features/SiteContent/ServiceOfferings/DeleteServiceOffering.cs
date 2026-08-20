using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.DeleteServiceOffering;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceOfferings;

public class DeleteServiceOffering : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/service-offerings/{id:guid}", Handle)
            .WithTags("SiteContent/ServiceOfferings")
            .RequireAuthorization()
            .WithName("DeleteServiceOffering")
            .WithDescription("Deletes a service offering card.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteServiceOfferingCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
