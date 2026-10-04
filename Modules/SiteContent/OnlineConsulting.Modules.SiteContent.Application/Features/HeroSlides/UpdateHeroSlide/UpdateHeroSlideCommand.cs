using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Abstractions;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.UpdateHeroSlide;

public record UpdateHeroSlideCommand(Guid Id, string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class UpdateHeroSlideHandler(IHeroSlideRepository repository, IStorageService storageService) : IRequestHandler<UpdateHeroSlideCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateHeroSlideCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Hero slide", request.Id);
        }

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.ImageUrl = storageService.ToStoredUrl(request.ImageUrl);
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        _ = await repository.UpdateAsync(entity, cancellationToken: cancellationToken);

        return Result.Success("Hero slide updated successfully.");
    }
}
