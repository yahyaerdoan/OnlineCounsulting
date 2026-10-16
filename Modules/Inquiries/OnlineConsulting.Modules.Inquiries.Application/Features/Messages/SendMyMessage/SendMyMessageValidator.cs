using FluentValidation;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SendMyMessage;

public class SendMyMessageValidator : AbstractValidator<SendMyMessageCommand>
{
    public SendMyMessageValidator()
    {
        _ = RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(4000).WithName("Message");
    }
}
