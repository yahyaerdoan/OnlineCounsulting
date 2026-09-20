using FluentValidation;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Baskets.SetBasketItemQuantity;

public class SetBasketItemQuantityValidator : AbstractValidator<SetBasketItemQuantityCommand>
{
    public SetBasketItemQuantityValidator()
    {
        _ = RuleFor(x => x.BasketItemId).NotEmpty();
        _ = RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
