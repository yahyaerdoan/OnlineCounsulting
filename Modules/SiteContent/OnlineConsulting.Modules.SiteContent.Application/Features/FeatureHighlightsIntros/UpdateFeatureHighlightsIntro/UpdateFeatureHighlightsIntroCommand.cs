using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.UpdateFeatureHighlightsIntro;

public record UpdateFeatureHighlightsIntroCommand(Guid Id, string Description, Guid? CoverMediaAssetId = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class UpdateFeatureHighlightsIntroHandler(IFeatureHighlightsIntroRepository repository) : IRequestHandler<UpdateFeatureHighlightsIntroCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateFeatureHighlightsIntroCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Feature highlights intro", request.Id);
        }

        entity.Description = request.Description;
        entity.CoverMediaAssetId = request.CoverMediaAssetId;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        _ = await repository.UpdateAsync(entity);

        return Result.Success("Feature highlights intro updated successfully.");
    }
}
