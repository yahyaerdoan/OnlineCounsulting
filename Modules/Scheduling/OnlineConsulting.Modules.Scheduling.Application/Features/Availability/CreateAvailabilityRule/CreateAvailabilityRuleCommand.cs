using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Scheduling.Application.Features.Constants;
using OnlineConsulting.Modules.Scheduling.Domain;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.CreateAvailabilityRule;

public record CreateAvailabilityRuleCommand(DayOfWeek DayOfWeek, TimeSpan StartTime, TimeSpan EndTime, int SlotDurationMinutes)
    : IRequest<OperationDataResult<Guid>>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [SchedulingOperationClaims.Admin, SchedulingOperationClaims.Write];
}

public class CreateAvailabilityRuleHandler(IAvailabilityRuleRepository repository) : IRequestHandler<CreateAvailabilityRuleCommand, OperationDataResult<Guid>>
{
    public async Task<OperationDataResult<Guid>> Handle(CreateAvailabilityRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = new AvailabilityRule
        {
            Id = Guid.NewGuid(),
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDurationMinutes = request.SlotDurationMinutes,
        };

        await repository.AddAsync(rule);

        return Result.Created(rule.Id, "Availability rule created successfully.");
    }
}
