using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Media.Application.Features.Constants;
using OnlineConsulting.Modules.Media.Application.Features.Rules;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Media.Application.Features.DeleteMediaAsset;

public record DeleteMediaAssetCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MediaOperationClaims.Admin, MediaOperationClaims.Write];
}

public class DeleteMediaAssetHandler(IMediaAssetRepository repository, IStorageService storageService) : IRequestHandler<DeleteMediaAssetCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteMediaAssetCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return MediaBusinessRules.NotFound(request.Id);

        // Only physically delete the file if this asset was actually stored by the currently active
        // provider - deleting via the wrong backend's API would either no-op or throw depending on
        // provider, neither of which is what we want here.
        if (entity.StorageProvider == storageService.ProviderName)
            await storageService.DeleteAsync(entity.Url, cancellationToken);

        await repository.DeleteAsync(entity);

        return Result.Success("Media asset deleted successfully.");
    }
}
