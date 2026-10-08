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
        _ = app.MapPut("/site-content/service-offerings/{id:guid}", Handle)
            .WithTags("SiteContent/ServiceOfferings")
            .RequireAuthorization()
            .WithName("UpdateServiceOffering")
            .WithDescription("Updates a service offering card.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdateServiceOfferingRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}

public record UpdateServiceOfferingRequest(string Title, string Description, string Icon, string? IconColor = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
{
    public UpdateServiceOfferingCommand ToCommand(Guid id) => new(id, Title, Description, Icon, IconColor, DisplayOrder, Metadata);
}
