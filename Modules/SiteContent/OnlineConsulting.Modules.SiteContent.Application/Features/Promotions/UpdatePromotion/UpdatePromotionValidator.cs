using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.UpdatePromotion;

public class UpdatePromotionValidator : AbstractValidator<UpdatePromotionCommand>
{
    public UpdatePromotionValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CtaText).MaximumLength(100);
        RuleFor(x => x.CtaUrl).MaximumLength(500);
    }
}
