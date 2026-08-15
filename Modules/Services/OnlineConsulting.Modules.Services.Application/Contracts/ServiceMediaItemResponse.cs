using OnlineConsulting.Modules.Services.Domain;

namespace OnlineConsulting.Modules.Services.Application.Contracts;

public record ServiceMediaItemResponse(Guid Id, Guid MediaAssetId, int DisplayOrder)
{
    public static ServiceMediaItemResponse FromDomain(ServiceMediaItem entity) => new(entity.Id, entity.MediaAssetId, entity.DisplayOrder);
}
