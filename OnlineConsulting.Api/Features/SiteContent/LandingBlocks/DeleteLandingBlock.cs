using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.DeleteLandingBlock;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LandingBlocks;

public class DeleteLandingBlock : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/site-content/landing-blocks/{id:guid}", Handle)
            .WithTags("SiteContent/LandingBlocks")
            .RequireAuthorization()
            .WithName("DeleteLandingBlock")
            .WithDescription("Deletes a platform landing block (SuperAdmin).")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteLandingBlockCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
