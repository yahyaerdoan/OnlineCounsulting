using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Equipment.Application.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.UpdateEquipmentItem;

public record UpdateEquipmentItemCommand(Guid Id, string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [EquipmentOperationClaims.Admin, EquipmentOperationClaims.Write];
}

public class UpdateEquipmentItemHandler(IEquipmentItemRepository repository) : IRequestHandler<UpdateEquipmentItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateEquipmentItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return Result.NotFound(string.Format(EquipmentMessages.EquipmentItemNotFoundFormat, request.Id));
        }

        entity.Type = request.Type;
        entity.Brand = request.Brand;
        entity.Model = request.Model;
        entity.SerialNumber = request.SerialNumber;
        entity.InstallDate = request.InstallDate;
        entity.WarrantyExpiresAt = request.WarrantyExpiresAt;
        entity.Notes = request.Notes;

        _ = await repository.UpdateAsync(entity);

        return Result.Success("Equipment item updated successfully.");
    }
}
