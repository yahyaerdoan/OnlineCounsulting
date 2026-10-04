using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.HeroSlides.CreateHeroSlide;

public record CreateHeroSlideCommand(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Add];
}

public class CreateHeroSlideHandler(IHeroSlideRepository repository, IStorageService storageService) : IRequestHandler<CreateHeroSlideCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateHeroSlideCommand request, CancellationToken cancellationToken)
    {
        var entity = new HeroSlide { Title = request.Title, Description = request.Description, ImageUrl = storageService.ToStoredUrl(request.ImageUrl), DisplayOrder = request.DisplayOrder, Metadata = MetadataSerializer.Serialize(request.Metadata) };

        _ = await repository.AddAsync(entity, cancellationToken: cancellationToken);

        return Result.Created(entity.Id, "Hero slide created successfully.");
    }
}
