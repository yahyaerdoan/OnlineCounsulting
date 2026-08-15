using OnlineConsulting.Modules.Media.Application.Common;
using OnlineConsulting.Modules.Media.Domain;

namespace OnlineConsulting.Modules.Media.Application.Contracts;

public record MediaAssetResponse(Guid Id, string Url, string? AltText, string ContentType, long SizeBytes, int? Width, int? Height, Dictionary<string, object>? Metadata)
{
    public static MediaAssetResponse FromDomain(MediaAsset entity) =>
        new(entity.Id, entity.Url, entity.AltText, entity.ContentType, entity.SizeBytes, entity.Width, entity.Height, MetadataSerializer.Deserialize(entity.Metadata));
}
