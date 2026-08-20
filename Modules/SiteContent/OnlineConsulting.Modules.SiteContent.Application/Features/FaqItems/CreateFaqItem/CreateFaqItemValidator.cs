using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.CreateFaqItem;

public class CreateFaqItemValidator : AbstractValidator<CreateFaqItemCommand>
{
    public CreateFaqItemValidator()
    {
        _ = RuleFor(x => x.ServiceId).NotEmpty();
        _ = RuleFor(x => x.Question).NotEmpty().MaximumLength(300);
        _ = RuleFor(x => x.Answer).NotEmpty().MaximumLength(2000);
    }
}
