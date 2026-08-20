using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Auth.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        _ = RuleFor(x => x.AccessToken).NotEmpty();
        _ = RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
