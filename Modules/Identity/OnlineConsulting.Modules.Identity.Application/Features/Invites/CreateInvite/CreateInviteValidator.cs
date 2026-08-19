using FluentValidation;

namespace OnlineConsulting.Modules.Identity.Application.Features.Invites.CreateInvite;

public class CreateInviteValidator : AbstractValidator<CreateInviteCommand>
{
    public CreateInviteValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
