using FluentValidation;

namespace OnlineConsulting.Modules.Tenancy.Application.Features.Signup;

public class SetTenantOwnerValidator : AbstractValidator<SetTenantOwnerCommand>
{
    public SetTenantOwnerValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.OwnerUserId).NotEmpty();
    }
}
