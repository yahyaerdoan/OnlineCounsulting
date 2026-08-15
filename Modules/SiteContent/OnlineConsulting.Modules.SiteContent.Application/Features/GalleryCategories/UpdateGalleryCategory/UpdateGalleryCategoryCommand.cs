using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Constants;
using OnlineConsulting.Modules.SiteContent.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.UpdateGalleryCategory;

public record UpdateGalleryCategoryCommand(Guid Id, string Name, string? Description = null) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdateGalleryCategoryHandler(IGalleryCategoryRepository repository) : IRequestHandler<UpdateGalleryCategoryCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateGalleryCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Gallery category", request.Id);

        entity.Name = request.Name;
        entity.Description = request.Description;

        await repository.UpdateAsync(entity);

        return Result.Success("Gallery category updated successfully.");
    }
}
