using FluentValidation;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.CreateWorkOrder;

public class CreateWorkOrderValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.TechnicianUserId).NotEmpty();
        RuleFor(x => x.PartsUsed).MaximumLength(2000);
        RuleFor(x => x.TechnicianNotes).MaximumLength(2000);
    }
}
