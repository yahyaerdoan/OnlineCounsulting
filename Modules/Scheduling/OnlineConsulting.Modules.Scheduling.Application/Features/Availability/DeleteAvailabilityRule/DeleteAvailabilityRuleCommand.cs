using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Abstractions;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.DeleteAvailabilityRule;

public record DeleteAvailabilityRuleCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write, SchedulingOperationClaims.Delete];
}

public class DeleteAvailabilityRuleHandler(IAvailabilityRuleRepository repository) : IRequestHandler<DeleteAvailabilityRuleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteAvailabilityRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await repository.GetAsync(r => r.Id == request.Id, cancellationToken: cancellationToken);
        if (rule is null)
        {
            return Result.NotFound(string.Format(SchedulingMessages.AvailabilityRuleNotFoundFormat, request.Id));
        }

        _ = await repository.DeleteAsync(rule, cancellationToken: cancellationToken);

        return Result.Success("Availability rule deleted successfully.");
    }
}
