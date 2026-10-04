using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.SharedKernel.Transactions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.DeleteGalleryItem;

/// <summary>Also deletes the item's GalleryItemCategory links first - unlike DeleteGalleryCategoryCommand, which leaves links orphaned.</summary>
public record DeleteGalleryItemCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest, ISiteContentTransactionRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Delete];
}

public class DeleteGalleryItemHandler(IGalleryItemRepository repository, IGalleryItemCategoryRepository categoryLinkRepository)
    : IRequestHandler<DeleteGalleryItemCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteGalleryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Gallery item", request.Id);
        }

        var links = await categoryLinkRepository.GetAllAsync(x => x.GalleryItemId == request.Id, cancellationToken: cancellationToken);
        foreach (var link in links)
        {
            _ = await categoryLinkRepository.DeleteAsync(link, cancellationToken: cancellationToken);
        }

        _ = await repository.DeleteAsync(entity, cancellationToken: cancellationToken);

        return Result.Success("Gallery item deleted successfully.");
    }
}
