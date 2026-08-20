using FluentValidation;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.AddBasketItem;

public class AddBasketItemValidator : AbstractValidator<AddBasketItemCommand>
{
    public AddBasketItemValidator()
    {
        _ = RuleFor(x => x.ServiceId).NotEmpty();
        _ = RuleFor(x => x.Quantity).GreaterThan(0);
        _ = RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        _ = RuleFor(x => x.TaxRate).InclusiveBetween(0, 100);
    }
}
