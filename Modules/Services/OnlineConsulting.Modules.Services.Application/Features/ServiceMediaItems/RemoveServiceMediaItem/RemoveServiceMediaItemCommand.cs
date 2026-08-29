using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Features.Constants;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Abstractions;
using OnlineConsulting.SharedKernel.Authorization;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.RemoveServiceMediaItem;

public record RemoveServiceMediaItemCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ServicesOperationClaims.Admin, ServicesOperationClaims.Write, ServicesOperationClaims.Update, GlobalOperationClaims.SuperAdmin];
}

public class RemoveServiceMediaItemHandler(IServiceMediaItemRepository repository) : IRequestHandler<RemoveServiceMediaItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(RemoveServiceMediaItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(m => m.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return Result.NotFound(string.Format(ServiceMessages.ServiceMediaItemNotFoundFormat, request.Id));
        }

        _ = await repository.DeleteAsync(entity);

        return Result.Success("Service media item removed successfully.");
    }
}
