using FluentValidation;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.AssignTechnician;

public class AssignTechnicianValidator : AbstractValidator<AssignTechnicianCommand>
{
    public AssignTechnicianValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.TechnicianUserId).NotEmpty();
    }
}
