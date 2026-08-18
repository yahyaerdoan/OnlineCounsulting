using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Equipment.Application.Common;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.DeleteEquipmentItem;

public record DeleteEquipmentItemCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [EquipmentOperationClaims.Admin, EquipmentOperationClaims.Write];
}

public class DeleteEquipmentItemHandler(IEquipmentItemRepository repository) : IRequestHandler<DeleteEquipmentItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteEquipmentItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return Result.NotFound(string.Format(EquipmentMessages.EquipmentItemNotFoundFormat, request.Id));

        await repository.DeleteAsync(entity);

        return Result.Success("Equipment item deleted successfully.");
    }
}
