using FluentValidation;
using OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.Constants;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Bundles.UpdateBundle;

public class UpdateBundleValidator : AbstractValidator<UpdateBundleCommand>
{
    public UpdateBundleValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.ModuleKeys).Must(k => k.Count > 0).WithMessage(BundleMessages.AtLeastOneModuleRequired);
    }
}
