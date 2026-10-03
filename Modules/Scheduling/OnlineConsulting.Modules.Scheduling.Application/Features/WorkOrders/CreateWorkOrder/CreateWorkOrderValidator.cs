using FluentValidation;

namespace OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.CreateWorkOrder;

public class CreateWorkOrderValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderValidator()
    {
        _ = RuleForEach(x => x.Charges).ChildRules(charge =>
        {
            _ = charge.RuleFor(c => c.Description).NotEmpty().MaximumLength(300);
            _ = charge.RuleFor(c => c.Quantity).GreaterThan(0).LessThanOrEqualTo(1000);
            _ = charge.RuleFor(c => c.UnitPrice).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1_000_000);
            _ = charge.RuleFor(c => c.TaxRate).InclusiveBetween(0, 100);
        });
        _ = RuleFor(x => x.AppointmentId).NotEmpty();
        _ = RuleFor(x => x.TechnicianUserId).NotEmpty();
        _ = RuleFor(x => x.PartsUsed).MaximumLength(2000);
        _ = RuleFor(x => x.TechnicianNotes).MaximumLength(2000);
    }
}
