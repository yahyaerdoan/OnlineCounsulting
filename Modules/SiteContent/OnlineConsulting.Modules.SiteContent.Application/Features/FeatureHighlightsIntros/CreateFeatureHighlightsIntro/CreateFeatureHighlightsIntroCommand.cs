using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Abstractions;
using OnlineConsulting.Modules.SiteContent.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.CreateFeatureHighlightsIntro;

public record CreateFeatureHighlightsIntroCommand(string Description, Guid? CoverMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Add];
}

public class CreateFeatureHighlightsIntroHandler(IFeatureHighlightsIntroRepository repository) : IRequestHandler<CreateFeatureHighlightsIntroCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateFeatureHighlightsIntroCommand request, CancellationToken cancellationToken)
    {
        var entity = new FeatureHighlightsIntro
        {
            Description = request.Description,
            CoverMediaAssetId = request.CoverMediaAssetId,
            DisplayOrder = request.DisplayOrder,
            Metadata = MetadataSerializer.Serialize(request.Metadata),
        };

        _ = await repository.AddAsync(entity);

        return Result.Created(entity.Id, "Feature highlights intro created successfully.");
    }
}
