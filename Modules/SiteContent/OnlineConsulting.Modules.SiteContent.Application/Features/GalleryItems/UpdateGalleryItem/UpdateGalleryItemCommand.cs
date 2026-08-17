using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.UpdateGalleryItem;

public record UpdateGalleryItemCommand(Guid Id, string Description, List<Guid> CategoryIds, Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdateGalleryItemHandler(IGalleryItemRepository repository, IGalleryItemCategoryRepository categoryLinkRepository)
    : IRequestHandler<UpdateGalleryItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateGalleryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Gallery item", request.Id);

        entity.Description = request.Description;
        entity.PhotoMediaAssetId = request.PhotoMediaAssetId;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        await repository.UpdateAsync(entity);

        var existingLinks = await categoryLinkRepository.GetListAsync(x => x.GalleryItemId == request.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        foreach (var link in existingLinks.Items)
        {
            await categoryLinkRepository.DeleteAsync(link);
        }

        foreach (var categoryId in request.CategoryIds.Distinct())
        {
            await categoryLinkRepository.AddAsync(new GalleryItemCategory { Id = Guid.NewGuid(), GalleryItemId = request.Id, GalleryCategoryId = categoryId });
        }

        return Result.Success("Gallery item updated successfully.");
    }
}
