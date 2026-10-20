using FluentValidation;
using OnlineConsulting.Modules.Scheduling.Application.Common;
using OnlineConsulting.SharedKernel.Identity;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.AssignTechnician;

public class AssignTechnicianValidator : AbstractValidator<AssignTechnicianCommand>
{
    public AssignTechnicianValidator(IStaffMemberReader staffReader)
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.TechnicianUserId).Cascade(CascadeMode.Stop).NotEmpty()
            .MustAsync(staffReader.IsStaffAsync).WithMessage(SchedulingMessages.TechnicianMustBeStaff);
    }
}
