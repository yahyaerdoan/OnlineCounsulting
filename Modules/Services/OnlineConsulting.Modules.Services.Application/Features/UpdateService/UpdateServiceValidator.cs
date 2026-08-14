using FluentValidation;

namespace OnlineConsulting.Modules.Services.Application.Features.UpdateService;

public class UpdateServiceValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(5).MaximumLength(2000);
        RuleFor(x => x.DetailedDescription).NotEmpty().MinimumLength(25);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.DiscountRate).InclusiveBetween(0, 100);
        RuleFor(x => x.TaxRate).InclusiveBetween(0, 100);
    }
}
