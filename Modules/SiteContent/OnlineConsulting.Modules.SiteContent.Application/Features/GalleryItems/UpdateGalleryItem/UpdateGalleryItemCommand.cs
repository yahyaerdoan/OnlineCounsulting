using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.UpdateGalleryItem;

/// <summary>Category links are replaced wholesale (delete all, then re-add CategoryIds) rather than diffed - simpler than reconciling adds/removes for a handful of rows per item.</summary>
public record UpdateGalleryItemCommand(Guid Id, string Description, List<Guid> CategoryIds, Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationResult>, ISecureAddRequest, ISiteContentTransactionRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class UpdateGalleryItemHandler(IGalleryItemRepository repository, IGalleryItemCategoryRepository categoryLinkRepository) : IRequestHandler<UpdateGalleryItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateGalleryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Gallery item", request.Id);
        }

        entity.Description = request.Description;
        entity.PhotoMediaAssetId = request.PhotoMediaAssetId;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        _ = await repository.UpdateAsync(entity, cancellationToken: cancellationToken);

        var links = await categoryLinkRepository.GetAllAsync(x => x.GalleryItemId == request.Id, withDeleted: true, enableTracking: true, cancellationToken: cancellationToken);
        var wanted = request.CategoryIds.ToHashSet();

        List<GalleryItemCategory> removed = [.. links.Where(l => l.DeletedDate is null && !wanted.Contains(l.GalleryCategoryId))];
        List<GalleryItemCategory> restored = [.. links.Where(l => l.DeletedDate is not null && wanted.Contains(l.GalleryCategoryId))];
        List<GalleryItemCategory> added = [.. wanted.Except(links.Select(l => l.GalleryCategoryId)).Select(categoryId => new GalleryItemCategory { GalleryItemId = request.Id, GalleryCategoryId = categoryId })];

        foreach (var link in restored)
        {
            link.DeletedDate = null;
            link.DeletedBy = null;
        }

        if (removed.Count > 0)
        {
            _ = await categoryLinkRepository.DeleteRangeAsync(removed, cancellationToken: cancellationToken);
        }

        if (restored.Count > 0)
        {
            _ = await categoryLinkRepository.UpdateRangeAsync(restored, cancellationToken);
        }

        if (added.Count > 0)
        {
            _ = await categoryLinkRepository.AddRangeAsync(added, cancellationToken);
        }

        return Result.Success("Gallery item updated successfully.");
    }
}
