using FluentValidation;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.UpdateTenantBranding;

public class UpdateTenantBrandingValidator : AbstractValidator<UpdateTenantBrandingCommand>
{
    public UpdateTenantBrandingValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
