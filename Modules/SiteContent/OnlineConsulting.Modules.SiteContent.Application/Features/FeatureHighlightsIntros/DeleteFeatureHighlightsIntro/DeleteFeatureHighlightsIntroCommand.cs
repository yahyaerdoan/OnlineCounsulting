using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FeatureHighlightsIntros.DeleteFeatureHighlightsIntro;

public record DeleteFeatureHighlightsIntroCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Delete];
}

public class DeleteFeatureHighlightsIntroHandler(IFeatureHighlightsIntroRepository repository) : IRequestHandler<DeleteFeatureHighlightsIntroCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteFeatureHighlightsIntroCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("Feature highlights intro", request.Id);
        }

        _ = await repository.DeleteAsync(entity);

        return Result.Success("Feature highlights intro deleted successfully.");
    }
}
