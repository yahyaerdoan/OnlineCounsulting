using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.CreateRole;

public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty();
    }
}
