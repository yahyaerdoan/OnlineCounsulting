using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.UpdateGalleryItem;

/// <summary>Category links are replaced wholesale (delete all, then re-add CategoryIds) rather than diffed - simpler than reconciling adds/removes for a handful of rows per item.</summary>
public record UpdateGalleryItemCommand(Guid Id, string Description, List<Guid> CategoryIds, Guid? PhotoMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationResult>, ISecureAddRequest, ISiteContentTransactionRequest
{
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class UpdateGalleryItemHandler(IGalleryItemRepository repository) : IRequestHandler<UpdateGalleryItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateGalleryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetWithCategoriesAsync(request.Id, cancellationToken: cancellationToken);

        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Gallery item", request.Id);
        }

        entity.Update(request.Description, request.PhotoMediaAssetId, request.DisplayOrder, MetadataSerializer.Serialize(request.Metadata), request.CategoryIds);

        _ = await repository.UpdateAsync(entity, cancellationToken: cancellationToken);

        return Result.Success("Gallery item updated successfully.");
    }
}
