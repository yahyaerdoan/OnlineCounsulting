using FluentValidation;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.AssignTechnician;

public class AssignTechnicianValidator : AbstractValidator<AssignTechnicianCommand>
{
    public AssignTechnicianValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TechnicianUserId).NotEmpty();
    }
}
