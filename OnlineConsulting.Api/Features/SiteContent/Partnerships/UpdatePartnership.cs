using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Partnerships.UpdatePartnership;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.Partnerships;

public class UpdatePartnership : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/site-content/partnerships/{id:guid}", Handle)
            .WithTags("SiteContent/Partnerships")
            .RequireAuthorization()
            .WithName("UpdatePartnership")
            .WithDescription("Updates a partnership showcase entry.");
    }

    private static async Task<IResult> Handle(Guid id, [FromBody] UpdatePartnershipCommand command, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(command with { Id = id });
        return result.ToEnvelopedResult(httpContext);
    }
}
