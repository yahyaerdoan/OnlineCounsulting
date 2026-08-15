using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.SiteContent.Application.Features.Constants;
using OnlineConsulting.Modules.SiteContent.Application.Features.Rules;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.DeleteServiceProcessStep;

public record DeleteServiceProcessStepCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SiteContentOperationClaims.Admin, SiteContentOperationClaims.Write];
}

public class DeleteServiceProcessStepHandler(IServiceProcessStepRepository repository) : IRequestHandler<DeleteServiceProcessStepCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteServiceProcessStepCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity is null)
            return SiteContentBusinessRules.NotFound("Service process step", request.Id);

        await repository.DeleteAsync(entity);

        return Result.Success("Service process step deleted successfully.");
    }
}
