using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.Abstractions;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryItems.DeleteGalleryItem;

public record DeleteGalleryItemCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
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

        var links = await categoryLinkRepository.GetListAsync(x => x.GalleryItemId == request.Id, size: RepositoryQuerySize.Unbounded, cancellationToken: cancellationToken);
        foreach (var link in links.Items)
        {
            _ = await categoryLinkRepository.DeleteAsync(link);
        }

        _ = await repository.DeleteAsync(entity);

        return Result.Success("Gallery item deleted successfully.");
    }
}
