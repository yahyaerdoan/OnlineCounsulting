using FluentValidation;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.CreateWorkOrder;

public class CreateWorkOrderValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderValidator()
    {
        _ = RuleFor(x => x.AppointmentId).NotEmpty();
        _ = RuleFor(x => x.TechnicianUserId).NotEmpty();
        _ = RuleFor(x => x.PartsUsed).MaximumLength(2000);
        _ = RuleFor(x => x.TechnicianNotes).MaximumLength(2000);
    }
}
