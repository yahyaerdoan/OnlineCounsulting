using FluentValidation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ReplyToMessage;

public class ReplyToMessageValidator : AbstractValidator<ReplyToMessageCommand>
{
    public ReplyToMessageValidator()
    {
        _ = RuleFor(x => x.ReplyBody).NotEmpty().MaximumLength(4000);
    }
}
