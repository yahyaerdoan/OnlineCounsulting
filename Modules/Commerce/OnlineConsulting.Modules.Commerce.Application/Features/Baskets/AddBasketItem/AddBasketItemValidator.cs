using FluentValidation;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.AddBasketItem;

public class AddBasketItemValidator : AbstractValidator<AddBasketItemCommand>
{
    public AddBasketItemValidator()
    {
        _ = RuleFor(x => x.ServiceId).NotEmpty();
        _ = RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
