using FluentValidation;
using OnlineConsulting.Modules.Services.Application.Features.Constants;

namespace OnlineConsulting.Modules.Services.Application.Features.CreateService;

public class CreateServiceValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceValidator()
    {
        _ = RuleFor(x => x.CategoryId).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        _ = RuleFor(x => x.DetailedDescription).NotEmpty().MinimumLength(25);
        _ = RuleFor(x => x.Price).GreaterThan(0);
        _ = RuleFor(x => x.DiscountRate).InclusiveBetween(0, 100);
        _ = RuleFor(x => x.TaxRate).InclusiveBetween(0, 100);
        _ = RuleFor(x => x.PriceType).Must(t => ServicePriceTypes.All.Contains(t)).WithMessage("Price type is not valid.");
        _ = RuleFor(x => x.PriceMax)
            .NotNull().WithMessage("Maximum price is required when price type is Range.")
            .GreaterThan(x => x.Price).WithMessage("Maximum price must be greater than the price.")
            .When(x => x.PriceType == ServicePriceTypes.Range);
        _ = RuleFor(x => x.PriceMax).Null().WithMessage("Maximum price must not be set unless price type is Range.").When(x => x.PriceType != ServicePriceTypes.Range);
    }
}
