using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Equipment.Application.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;
using OnlineConsulting.Modules.Equipment.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.CreateEquipmentItem;

/// <summary>Admin/technician-recorded, not customer self-service - equipment records are meant to reflect what a technician actually found/installed on site.</summary>
public record CreateEquipmentItemCommand(Guid UserId, string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [EquipmentOperationClaims.Admin, EquipmentOperationClaims.Write];
}

public class CreateEquipmentItemHandler(IEquipmentItemRepository repository) : IRequestHandler<CreateEquipmentItemCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateEquipmentItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new EquipmentItem
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Type = request.Type,
            Brand = request.Brand,
            Model = request.Model,
            SerialNumber = request.SerialNumber,
            InstallDate = request.InstallDate,
            WarrantyExpiresAt = request.WarrantyExpiresAt,
            Notes = request.Notes,
        };

        _ = await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Equipment item created successfully.");
    }
}
