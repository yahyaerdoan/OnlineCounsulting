using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Common;
using OnlineConsulting.Modules.SiteContent.Application.Features.Constants;
using OnlineConsulting.Modules.SiteContent.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.UpdateServiceProcessStep;

public record UpdateServiceProcessStepCommand(Guid Id, string Title, string Description, string Icon, string? IconColor = null, int DisplayOrder = 0, Dictionary<string, object>? Metadata = null)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class UpdateServiceProcessStepHandler(IServiceProcessStepRepository repository) : IRequestHandler<UpdateServiceProcessStepCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateServiceProcessStepCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Service process step", request.Id);

        entity.Title = request.Title;
        entity.Description = request.Description;
        entity.Icon = request.Icon;
        entity.IconColor = request.IconColor;
        entity.DisplayOrder = request.DisplayOrder;
        entity.Metadata = MetadataSerializer.Serialize(request.Metadata);

        await repository.UpdateAsync(entity);

        return Result.Success("Service process step updated successfully.");
    }
}
