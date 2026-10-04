using FluentValidation;
using OnlineConsulting.Modules.Scheduling.Domain;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CreateAppointment;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentValidator()
    {
        _ = RuleFor(x => x.ScheduledStart).GreaterThan(DateTimeOffset.UtcNow);
        _ = RuleFor(x => x.ScheduledEnd).GreaterThan(x => x.ScheduledStart);
        _ = RuleFor(x => x.CustomerNote).MaximumLength(1000);
        _ = RuleFor(x => x.ServiceAddress).MaximumLength(500);
        _ = RuleFor(x => x.MeetingType).Must(type => AppointmentMeetingTypes.All.Contains(type)).WithMessage("Choose an in-person visit or an online meeting.");
        _ = RuleFor(x => x.Topic).NotEmpty().WithMessage("Tell us what the meeting is about.").MaximumLength(200)
            .When(x => x.MeetingType == AppointmentMeetingTypes.Online);
        _ = RuleFor(x => x.ServiceAddress).NotEmpty().WithMessage("Add the address where the visit will take place.")
            .When(x => x.MeetingType == AppointmentMeetingTypes.InPerson);
    }
}
