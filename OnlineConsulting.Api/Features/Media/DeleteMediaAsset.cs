using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Media.Application.Features.MediaAssets.DeleteMediaAsset;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Media;

public class DeleteMediaAsset : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapDelete("/media/{id:guid}", Handle)
            .WithTags("Media")
            .RequireAuthorization()
            .WithName("DeleteMediaAsset")
            .WithDescription("Deletes a media asset and its underlying file.")
            .ProducesEnveloped();
    }

    private static async Task<IResult> Handle(Guid id, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new DeleteMediaAssetCommand(id));
        return result.ToEnvelopedResult(httpContext);
    }
}
