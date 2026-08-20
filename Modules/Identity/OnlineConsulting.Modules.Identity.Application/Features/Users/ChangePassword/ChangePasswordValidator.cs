using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.CurrentPassword).NotEmpty();
        _ = RuleFor(x => x.NewPassword).NotEmpty();
    }
}
