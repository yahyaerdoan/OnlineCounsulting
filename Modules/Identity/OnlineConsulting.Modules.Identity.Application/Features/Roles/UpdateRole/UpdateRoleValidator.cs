using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Roles.UpdateRole;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
