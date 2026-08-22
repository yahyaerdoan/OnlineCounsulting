using FluentValidation;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.ModuleOfferings.UpdateModuleOffering;

public class UpdateModuleOfferingValidator : AbstractValidator<UpdateModuleOfferingCommand>
{
    public UpdateModuleOfferingValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
