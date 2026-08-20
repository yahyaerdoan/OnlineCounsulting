using FluentValidation;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Promotions.UpdatePromotion;

public class UpdatePromotionValidator : AbstractValidator<UpdatePromotionCommand>
{
    public UpdatePromotionValidator()
    {
        _ = RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        _ = RuleFor(x => x.CtaText).MaximumLength(100);
        _ = RuleFor(x => x.CtaUrl).MaximumLength(500);
    }
}
