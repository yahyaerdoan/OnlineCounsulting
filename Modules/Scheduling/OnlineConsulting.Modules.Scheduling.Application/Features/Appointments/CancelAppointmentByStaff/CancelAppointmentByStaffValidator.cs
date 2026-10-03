using FluentValidation;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.CancelAppointmentByStaff;

public class CancelAppointmentByStaffValidator : AbstractValidator<CancelAppointmentByStaffCommand>
{
    public CancelAppointmentByStaffValidator()
    {
        _ = RuleFor(x => x.Reason).MaximumLength(500);
    }
}
