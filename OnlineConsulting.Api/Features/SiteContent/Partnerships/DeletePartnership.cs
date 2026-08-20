using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.DeletePartnership;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Partnerships;

public class DeletePartnership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/api/site-content/partnerships/{id:guid}", Handle)
            .WithTags("SiteContent/Partnerships")
            .RequireAuthorization()
            .WithName("DeletePartnership")
            .WithDescription("Deletes a partnership showcase entry.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeletePartnershipCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
