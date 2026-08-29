using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Media.Application.Features.GetMediaAsset;
using ResultHandler.AspNetCore.Extensions;
using ResultHandler.Facade;

namespace OnlineConsulting.Api.Features.Media;

public class GetMediaAsset : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/api/media/{id:guid}", Handle)
            .WithTags("Media")
            .WithName("GetMediaAsset")
            .WithDescription("Returns a single media asset by id. Public - no login required.");
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetMediaAssetQuery(id));
        if (!result.IsSuccessful)
        {
            return result.ToEnvelopedResult(httpContext);
        }

        var resolved = result.Data with { Url = MediaUrlResolver.Resolve(result.Data.Url, httpContext) };
        return Result.Success(resolved, result.Title).ToEnvelopedResult(httpContext);
    }
}
