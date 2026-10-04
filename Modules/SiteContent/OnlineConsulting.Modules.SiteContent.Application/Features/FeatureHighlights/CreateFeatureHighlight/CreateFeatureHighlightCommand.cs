using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using OnlineConsulting.SharedKernel.Media;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.CreateFeatureHighlight;

public record CreateFeatureHighlightCommand(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Add];
}

public class CreateFeatureHighlightHandler(IFeatureHighlightRepository repository, IStorageService storageService) : IRequestHandler<CreateFeatureHighlightCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateFeatureHighlightCommand request, CancellationToken cancellationToken)
    {
        var entity = new FeatureHighlight { Title = request.Title, Description = request.Description, ImageUrl = storageService.ToStoredUrl(request.ImageUrl), DisplayOrder = request.DisplayOrder, Metadata = MetadataSerializer.Serialize(request.Metadata) };

        _ = await repository.AddAsync(entity, cancellationToken: cancellationToken);

        return Result.Created(entity.Id, "Feature highlight created successfully.");
    }
}
