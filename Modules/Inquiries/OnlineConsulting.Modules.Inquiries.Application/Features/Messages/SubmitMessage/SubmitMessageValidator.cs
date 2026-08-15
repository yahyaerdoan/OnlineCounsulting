using FluentValidation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SubmitMessage;

public class SubmitMessageValidator : AbstractValidator<SubmitMessageCommand>
{
    public SubmitMessageValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}
