using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain.Gallery;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.GalleryCategories.CreateGalleryCategory;

public record CreateGalleryCategoryCommand(string Name, string? Description = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreateGalleryCategoryHandler(IGalleryCategoryRepository repository) : IRequestHandler<CreateGalleryCategoryCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateGalleryCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = new GalleryCategory { Id = Guid.NewGuid(), Name = request.Name, Description = request.Description };

        _ = await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Gallery category created successfully.");
    }
}
