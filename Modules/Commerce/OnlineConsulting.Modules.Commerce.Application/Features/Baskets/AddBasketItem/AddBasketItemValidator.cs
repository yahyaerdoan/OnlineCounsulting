using FluentValidation;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.AddBasketItem;

public class AddBasketItemValidator : AbstractValidator<AddBasketItemCommand>
{
    public AddBasketItemValidator()
    {
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 100);
    }
}
