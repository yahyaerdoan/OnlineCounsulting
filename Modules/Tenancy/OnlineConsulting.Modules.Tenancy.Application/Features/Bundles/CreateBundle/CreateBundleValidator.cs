using FluentValidation;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Constants;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.CreateBundle;

public class CreateBundleValidator : AbstractValidator<CreateBundleCommand>
{
    public CreateBundleValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.ModuleKeys).Must(k => k.Count > 0).WithMessage(BundleMessages.AtLeastOneModuleRequired);
    }
}
