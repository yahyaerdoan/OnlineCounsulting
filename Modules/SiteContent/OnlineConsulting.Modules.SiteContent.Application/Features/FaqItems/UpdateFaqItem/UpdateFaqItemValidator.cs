using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.FaqItems.UpdateFaqItem;

public class UpdateFaqItemValidator : AbstractValidator<UpdateFaqItemCommand>
{
    public UpdateFaqItemValidator()
    {
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.Question).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Answer).NotEmpty().MaximumLength(2000);
    }
}
