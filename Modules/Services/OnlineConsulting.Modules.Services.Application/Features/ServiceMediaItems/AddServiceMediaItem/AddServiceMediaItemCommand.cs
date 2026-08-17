using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Application.Features.Constants;
using OnlineConsulting.Modules.Services.Application.Features.Rules;
using OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.Abstractions;
using OnlineConsulting.Modules.Services.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using ResultHandler.Functional;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Services.Application.Features.ServiceMediaItems.AddServiceMediaItem;

/// <summary>Attaches an already-uploaded MediaAsset (photo or video, told apart by its ContentType) to a Service's gallery. Upload itself goes through the Media module's own UploadMediaAsset first - not duplicated here.</summary>
public record AddServiceMediaItemCommand(Guid ServiceId, Guid MediaAssetId, int DisplayOrder = 0) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ServicesOperationClaims.Admin, ServicesOperationClaims.Write, ServicesOperationClaims.Update];
}

public class AddServiceMediaItemHandler(IServiceMediaItemRepository repository, IServiceRepository serviceRepository)
    : IRequestHandler<AddServiceMediaItemCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(AddServiceMediaItemCommand request, CancellationToken cancellationToken)
    {
        var serviceExists = await serviceRepository.AnyAsync(s => s.Id == request.ServiceId, cancellationToken: cancellationToken);
        if (!serviceExists)
            return ServiceBusinessRules.ServiceNotFound(request.ServiceId).ToErrorDataResult<Guid>();

        var entity = new ServiceMediaItem { Id = Guid.NewGuid(), ServiceId = request.ServiceId, MediaAssetId = request.MediaAssetId, DisplayOrder = request.DisplayOrder };

        await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Service media item added successfully.");
    }
}
