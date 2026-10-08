using OnlineConsulting.Modules.Equipment.Domain;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Contracts;

public record EquipmentItemResponse(Guid Id, Guid UserId, string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes)
{
    public static EquipmentItemResponse FromDomain(EquipmentItem entity) =>
        new(entity.Id, entity.UserId, entity.Type, entity.Brand, entity.Model, entity.SerialNumber, entity.InstallDate, entity.WarrantyExpiresAt, entity.Notes);
}
