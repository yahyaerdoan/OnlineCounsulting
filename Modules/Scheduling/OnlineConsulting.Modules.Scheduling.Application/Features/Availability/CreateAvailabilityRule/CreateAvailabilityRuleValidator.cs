using FluentValidation;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Availability.CreateAvailabilityRule;

public class CreateAvailabilityRuleValidator : AbstractValidator<CreateAvailabilityRuleCommand>
{
    public CreateAvailabilityRuleValidator()
    {
        _ = RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
        _ = RuleFor(x => x.SlotDurationMinutes).InclusiveBetween(5, 480);
    }
}
