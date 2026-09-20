using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.Token).NotEmpty();

        _ = RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$")
            .WithMessage("Password must contain an uppercase letter, a lowercase letter, a digit and a symbol.");
    }
}
