using FluentValidation;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CreateAppointment;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.ScheduledStart).GreaterThan(DateTimeOffset.UtcNow);
        RuleFor(x => x.ScheduledEnd).GreaterThan(x => x.ScheduledStart);
        RuleFor(x => x.CustomerNote).MaximumLength(1000);
        RuleFor(x => x.ServiceAddress).MaximumLength(500);
    }
}
