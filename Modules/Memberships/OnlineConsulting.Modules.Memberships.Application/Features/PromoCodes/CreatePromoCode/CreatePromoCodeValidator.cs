using FluentValidation;
using OnlineConsulting.Modules.Memberships.Application.Common;

namespace OnlineConsulting.Modules.Memberships.Application.Features.PromoCodes.CreatePromoCode;

public class CreatePromoCodeValidator : AbstractValidator<CreatePromoCodeCommand>
{
    public CreatePromoCodeValidator()
    {
        _ = RuleFor(x => x.Code).NotEmpty().MaximumLength(40).Matches("^[A-Za-z0-9-]+$").WithMessage("Code may only contain letters, numbers, and dashes.");
        _ = RuleFor(x => x.DiscountType).Must(t => t is PromoCodeDiscountTypes.Percent or PromoCodeDiscountTypes.Fixed).WithMessage("Discount type must be Percent or Fixed.");
        _ = RuleFor(x => x.DiscountValue).GreaterThan(0);
        _ = RuleFor(x => x.DiscountValue).LessThanOrEqualTo(100).When(x => x.DiscountType == PromoCodeDiscountTypes.Percent).WithMessage("Percent discount value must be between 0 and 100.");
        _ = RuleFor(x => x.MaxRedemptions).GreaterThan(0).When(x => x.MaxRedemptions is not null);
        _ = RuleFor(x => x.ExpiresAt).GreaterThan(DateTimeOffset.UtcNow).When(x => x.ExpiresAt is not null).WithMessage("Expiration date must be in the future.");
    }
}
