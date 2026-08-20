using FluentValidation;
using OnlineConsulting.Modules.Services.Application.Features.Constants;

namespace OnlineConsulting.Modules.Services.Application.Features.UpdateService;

public class UpdateServiceValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.CategoryId).NotEmpty();
        _ = RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(200);
        _ = RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        _ = RuleFor(x => x.DetailedDescription).NotEmpty().MinimumLength(25);
        _ = RuleFor(x => x.Price).GreaterThan(0);
        _ = RuleFor(x => x.DiscountRate).InclusiveBetween(0, 100);
        _ = RuleFor(x => x.TaxRate).InclusiveBetween(0, 100);
        _ = RuleFor(x => x.PriceType).Must(t => ServicePriceTypes.All.Contains(t));
        _ = RuleFor(x => x.PriceMax).NotNull().GreaterThan(x => x.Price).When(x => x.PriceType == ServicePriceTypes.Range);
        _ = RuleFor(x => x.PriceMax).Null().When(x => x.PriceType != ServicePriceTypes.Range);
    }
}
