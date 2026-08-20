using FluentValidation;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

public class SetTenantOwnerValidator : AbstractValidator<SetTenantOwnerCommand>
{
    public SetTenantOwnerValidator()
    {
        _ = RuleFor(x => x.TenantId).NotEmpty();
        _ = RuleFor(x => x.OwnerUserId).NotEmpty();
    }
}
