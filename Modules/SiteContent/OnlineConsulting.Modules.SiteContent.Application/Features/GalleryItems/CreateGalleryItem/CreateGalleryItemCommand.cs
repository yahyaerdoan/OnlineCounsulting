using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.CreateGalleryItem;

/// <summary>CategoryIds is required to have at least one entry (CreateGalleryItemValidator) - preserves the legacy business rule that a gallery item must be tagged.</summary>
public record CreateGalleryItemCommand(string Description, List<Guid> CategoryIds, Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreateGalleryItemHandler(IGalleryItemRepository repository, IGalleryItemCategoryRepository categoryLinkRepository)
    : IRequestHandler<CreateGalleryItemCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateGalleryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new GalleryItem
        {
            Id = Guid.NewGuid(),
            Description = request.Description,
            PhotoMediaAssetId = request.PhotoMediaAssetId,
            DisplayOrder = request.DisplayOrder,
            Metadata = MetadataSerializer.Serialize(request.Metadata),
        };

        await repository.AddAsync(entity);

        foreach (var categoryId in request.CategoryIds.Distinct())
        {
            await categoryLinkRepository.AddAsync(new GalleryItemCategory { Id = Guid.NewGuid(), GalleryItemId = entity.Id, GalleryCategoryId = categoryId });
        }

        return Result.Created(entity.Id, "Gallery item created successfully.");
    }
}
