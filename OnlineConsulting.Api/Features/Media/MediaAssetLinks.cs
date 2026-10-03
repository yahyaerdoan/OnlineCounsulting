using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Media.Application.Features.MediaAssets.Contracts;
using OnlineConsulting.Modules.Media.Application.Features.MediaAssets.DeleteMediaAsset;

namespace OnlineConsulting.Api.Features.Media;

public sealed class MediaAssetLinks : LinkProvider<MediaAssetResponse>
{
    protected override void AddLinks(MediaAssetResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetMediaAsset", new { id = resource.Id })
            .AddCustomIf(links.User.CanSend<DeleteMediaAssetCommand>(), Rels.Delete, "DeleteMediaAsset", HttpMethods.Delete, new { id = resource.Id });
}
