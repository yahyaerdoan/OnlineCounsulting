using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.GetAllLandingBlocks;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.SiteContent.LandingBlocks;

public class GetAllLandingBlocks : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/site-content/landing-blocks", Handle)
            .WithTags("SiteContent/LandingBlocks")
            .WithName("GetAllLandingBlocks")
            .WithDescription("Returns every platform landing block in display order. Public - no login required.")
            .ProducesEnveloped<List<LandingBlockResponse>>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetAllLandingBlocksQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
