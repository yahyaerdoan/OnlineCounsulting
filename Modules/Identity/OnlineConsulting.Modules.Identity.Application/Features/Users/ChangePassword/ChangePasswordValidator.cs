using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Users.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.CurrentPassword).NotEmpty();
        _ = RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")
            .WithMessage("Password must contain an uppercase letter, a lowercase letter, a digit and a symbol.");
    }
}
