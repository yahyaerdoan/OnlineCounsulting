using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.Abstractions;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlights.CreateFeatureHighlight;

public record CreateFeatureHighlightCommand(string Title, string Description, string ImageUrl, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class CreateFeatureHighlightHandler(IFeatureHighlightRepository repository) : IRequestHandler<CreateFeatureHighlightCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateFeatureHighlightCommand request, CancellationToken cancellationToken)
    {
        var entity = new FeatureHighlight { Id = Guid.NewGuid(), Title = request.Title, Description = request.Description, ImageUrl = request.ImageUrl, DisplayOrder = request.DisplayOrder, Metadata = MetadataSerializer.Serialize(request.Metadata) };

        await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Feature highlight created successfully.");
    }
}
