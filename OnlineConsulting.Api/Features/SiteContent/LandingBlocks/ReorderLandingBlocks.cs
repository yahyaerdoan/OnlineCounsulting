using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.ReorderLandingBlocks;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LandingBlocks;

public class ReorderLandingBlocks : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPut("/site-content/landing-blocks/order", Handle)
            .WithTags("SiteContent/LandingBlocks")
            .RequireAuthorization()
            .WithName("ReorderLandingBlocks")
            .WithDescription("Sets the order of one landing section's entries in a single save (SuperAdmin).")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle([FromBody] ReorderLandingBlocksRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record ReorderLandingBlocksRequest(string Section, List<Guid> OrderedIds)
{
    public ReorderLandingBlocksCommand ToCommand() => new(Section, OrderedIds);
}
