using FluentValidation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SubmitMessage;

public class SubmitMessageValidator : AbstractValidator<SubmitMessageCommand>
{
    public SubmitMessageValidator()
    {
        _ = RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        _ = RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}
