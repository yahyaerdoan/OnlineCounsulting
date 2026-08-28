using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceAreas.UpdateServiceArea;

/// <summary>Never touches Slug - the SEO URL stays stable once published, even if Name changes.</summary>
public record UpdateServiceAreaCommand(Guid Id, string Name, string State, string? IntroText, int DisplayOrder = 0)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write, SiteContentOperationClaims.Update];
}

public class UpdateServiceAreaHandler(IServiceAreaRepository repository) : IRequestHandler<UpdateServiceAreaCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateServiceAreaCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
        {
            return SiteContentBusinessRules.NotFound("ServiceArea", request.Id);
        }

        entity.Name = request.Name;
        entity.State = request.State;
        entity.IntroText = request.IntroText;
        entity.DisplayOrder = request.DisplayOrder;

        _ = await repository.UpdateAsync(entity);

        return Result.Success("Service area updated successfully.");
    }
}
