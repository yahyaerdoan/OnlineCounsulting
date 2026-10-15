using FluentValidation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.ReplyToMyMessage;

public class ReplyToMyMessageValidator : AbstractValidator<ReplyToMyMessageCommand>
{
    public ReplyToMyMessageValidator()
    {
        _ = RuleFor(x => x.ReplyBody).NotEmpty().MaximumLength(4000);
    }
}
