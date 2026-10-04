using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.GetAvailabilityRules;

public record GetAvailabilityRulesQuery : IRequest<OperationDataResult<List<AvailabilityRuleResponse>>>;

public class GetAvailabilityRulesHandler(IAvailabilityRuleRepository repository)
    : IRequestHandler<GetAvailabilityRulesQuery, OperationDataResult<List<AvailabilityRuleResponse>>>
{
    public async Task<OperationDataResult<List<AvailabilityRuleResponse>>> Handle(GetAvailabilityRulesQuery request, CancellationToken cancellationToken)
    {
        var rules = await repository.GetAllAsync(orderBy: q => q.OrderBy(r => r.DayOfWeek).ThenBy(r => r.StartTime), cancellationToken: cancellationToken);

        var response = rules.Select(AvailabilityRuleResponse.FromDomain).ToList();

        return Result.Success(response, "Availability rules retrieved successfully.");
    }
}
