using FluentValidation;
using OnlineConsulting.SharedKernel.Payments;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.CreateModuleOffering;

public class CreateModuleOfferingValidator : AbstractValidator<CreateModuleOfferingCommand>
{
    public CreateModuleOfferingValidator()
    {
        _ = RuleFor(x => x.Key).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Price).GreaterThan(0);
        _ = RuleFor(x => x.BillingCycle).Must(c => c is BillingCycles.Monthly or BillingCycles.Annual);
    }
}
