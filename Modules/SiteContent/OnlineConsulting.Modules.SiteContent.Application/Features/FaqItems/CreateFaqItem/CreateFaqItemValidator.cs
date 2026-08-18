using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.CreateFaqItem;

public class CreateFaqItemValidator : AbstractValidator<CreateFaqItemCommand>
{
    public CreateFaqItemValidator()
    {
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.Question).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Answer).NotEmpty().MaximumLength(2000);
    }
}
