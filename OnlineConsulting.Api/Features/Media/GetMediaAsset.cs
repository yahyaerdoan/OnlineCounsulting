using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Media.Application.Features.GetMediaAsset;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Media;

public class GetMediaAsset : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/media/{id:guid}", Handle)
            .WithTags("Media")
            .WithName("GetMediaAsset")
            .WithDescription("Returns a single media asset by id. Public - no login required.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetMediaAssetQuery(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
