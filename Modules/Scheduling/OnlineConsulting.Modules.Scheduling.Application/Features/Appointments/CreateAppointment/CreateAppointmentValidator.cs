using FluentValidation;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CreateAppointment;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentValidator()
    {
        _ = RuleFor(x => x.ScheduledStart).GreaterThan(DateTimeOffset.UtcNow);
        _ = RuleFor(x => x.ScheduledEnd).GreaterThan(x => x.ScheduledStart);
        _ = RuleFor(x => x.CustomerNote).MaximumLength(1000);
        _ = RuleFor(x => x.ServiceAddress).MaximumLength(500);
    }
}
