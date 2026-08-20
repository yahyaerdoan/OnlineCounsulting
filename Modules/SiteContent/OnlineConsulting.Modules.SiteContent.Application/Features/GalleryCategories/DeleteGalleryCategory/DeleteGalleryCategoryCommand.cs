using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.DeleteGalleryCategory;

public record DeleteGalleryCategoryCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
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

        _ = await repository.DeleteAsync(entity);

        return Result.Success("Gallery category deleted successfully.");
    }
}
