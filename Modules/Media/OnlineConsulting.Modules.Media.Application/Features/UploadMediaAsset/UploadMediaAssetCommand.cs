using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Media.Application.Abstractions;
using OnlineConsulting.Modules.Media.Application.Common;
using OnlineConsulting.Modules.Media.Application.Features.Constants;
using OnlineConsulting.Modules.Media.Domain;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Media.Application.Features.UploadMediaAsset;

/// <summary>FileStream/FileName/ContentType instead of a framework file type (IFormFile etc.) - the Api layer converts whatever transport it received into these primitives, keeping this command transport-agnostic.</summary>
public record UploadMediaAssetCommand(Stream FileStream, string FileName, string ContentType, string? AltText, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [MediaOperationClaims.Admin, MediaOperationClaims.Write];
}

public class UploadMediaAssetHandler(IMediaAssetRepository repository, IStorageService storageService) : IRequestHandler<UploadMediaAssetCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(UploadMediaAssetCommand request, CancellationToken cancellationToken)
    {
        var uploadResult = await storageService.UploadAsync(request.FileStream, request.FileName, request.ContentType, cancellationToken);

        var entity = new MediaAsset
        {
            Id = Guid.NewGuid(),
            Url = uploadResult.Url,
            AltText = request.AltText,
            ContentType = request.ContentType,
            SizeBytes = uploadResult.SizeBytes,
            Width = uploadResult.Width,
            Height = uploadResult.Height,
            StorageProvider = storageService.ProviderName,
            Metadata = MetadataSerializer.Serialize(request.Metadata),
        };

        await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Media asset uploaded successfully.");
    }
}
