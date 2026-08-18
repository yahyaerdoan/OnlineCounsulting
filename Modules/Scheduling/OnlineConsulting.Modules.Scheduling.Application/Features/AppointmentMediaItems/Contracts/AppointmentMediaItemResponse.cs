using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.Contracts;

public record AppointmentMediaItemResponse(Guid Id, Guid MediaAssetId, int DisplayOrder)
{
    public static AppointmentMediaItemResponse FromDomain(AppointmentMediaItem entity) => new(entity.Id, entity.MediaAssetId, entity.DisplayOrder);
}
