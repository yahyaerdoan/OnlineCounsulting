using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.DeleteGalleryCategory;

/// <summary>Does not clean up GalleryItemCategory links pointing at this category - GetAllGalleryItemsQuery/Paged filter orphaned links out defensively instead.</summary>
public record DeleteGalleryCategoryCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Delete];
}

public class DeleteGalleryCategoryHandler(IGalleryCategoryRepository repository) : IRequestHandler<DeleteGalleryCategoryCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteGalleryCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Gallery category", request.Id);
        }

        _ = await repository.DeleteAsync(entity, cancellationToken: cancellationToken);

        return Result.Success("Gallery category deleted successfully.");
    }
}
