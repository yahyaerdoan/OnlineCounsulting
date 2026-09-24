using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Media.Application.Abstractions;
using OnlineConsulting.Modules.Media.Application.Features.Constants;
using OnlineConsulting.Modules.Media.Application.Features.Rules;
using OnlineConsulting.SharedKernel.Authorization;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Media.Application.Features.DeleteMediaAsset;

public record DeleteMediaAssetCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MediaOperationClaims.Admin, MediaOperationClaims.Write, MediaOperationClaims.Delete, GlobalOperationClaims.SuperAdmin];
}

/// <summary>Deletes a media asset; the file is only removed from storage if the active provider matches the one that originally stored it, since another backend's delete API would no-op or throw.</summary>
public class DeleteMediaAssetHandler(IMediaAssetRepository repository, IStorageService storageService) : IRequestHandler<DeleteMediaAssetCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteMediaAssetCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return MediaBusinessRules.NotFound(request.Id);
        }

        if (entity.StorageProvider == storageService.ProviderName)
        {
            await storageService.DeleteAsync(entity.Url, cancellationToken);
        }

        _ = await repository.DeleteAsync(entity);

        return Result.Success("Media asset deleted successfully.");
    }
}
