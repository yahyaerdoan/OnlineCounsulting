using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.CreateGalleryItem;

/// <summary>CategoryIds is required to have at least one entry (CreateGalleryItemValidator) - preserves the legacy business rule that a gallery item must be tagged.</summary>
public record CreateGalleryItemCommand(string Description, List<Guid> CategoryIds, Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest, ISiteContentTransactionRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Add];
}

public class CreateGalleryItemHandler(IGalleryItemRepository repository) : IRequestHandler<CreateGalleryItemCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateGalleryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = GalleryItem.Create(request.Description, request.PhotoMediaAssetId, request.DisplayOrder, MetadataSerializer.Serialize(request.Metadata), request.CategoryIds);

        _ = await repository.AddAsync(entity, cancellationToken: cancellationToken);

        return Result.Created(entity.Id, "Gallery item created successfully.");
    }
}
